namespace DrawingROIApp
{
    public partial class Form1 : Form
    {
        // ----------------------------------------------------------------
        // 定数
        // ----------------------------------------------------------------
        // プロジェクト直下（DrawingROIApp.csproj と同じ階層）の Data フォルダを参照する。
        // 実行ファイルの場所から親フォルダをたどって探すため、bin 配下には Data を作らない。
        private static readonly string DataFolderPath = FindDataFolder();

        private static string FindDataFolder()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "DrawingROIApp.csproj")))
                    return Path.Combine(dir.FullName, "Data");
                dir = dir.Parent;
            }
            return Path.Combine(AppContext.BaseDirectory, "Data");
        }

        private static readonly string CameraSettingFilePath = Path.Combine(DataFolderPath, "settings", "setting-camera.txt");
        private static readonly string RoiSaveFolderPath = Path.Combine(DataFolderPath, "settings", "roi");
        private static readonly string SampleFolderPath = Path.Combine(DataFolderPath, "sample");

        // ----------------------------------------------------------------
        // データクラス
        // ----------------------------------------------------------------
        private record CameraInfo(string IpAddress, string Label)
        {
            public string ImagePath =>
                Path.Combine(SampleFolderPath, $"average-template_{IpAddress}-4.jpg");

            public string MasterDataPath =>
                Path.Combine(SampleFolderPath, $"{IpAddress}_Combined_4.txt");
        }

        private record RoiEntry(int X, int Y, int W, int H);

        // ----------------------------------------------------------------
        // 状態変数
        // ----------------------------------------------------------------
        private List<CameraInfo> _cameras = new();
        private CameraInfo? _selectedCamera;
        private Bitmap? _currentImage;
        private List<RoiEntry> _roiList = new();

        // BBOX描画中状態
        private bool _isDrawing = false;
        private Point _drawStart;
        private Point _drawCurrent;

        // LED表示状態
        private bool _isLedVisible = false;
        private List<RoiEntry> _masterRoiList = new();

        // ズーム状態（1.0 = フィット表示）
        private float _zoomFactor = 1.0f;
        private PointF _viewOffset = PointF.Empty;

        // 選択中ROIインデックス（-1 = 未選択）
        private int _selectedRoiIndex = -1;

        // ----------------------------------------------------------------
        // コンストラクタ
        // ----------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
            SetupDataGridView();
            LoadCameraList();

            // 描画のちらつき防止（PictureBox.DoubleBuffered は protected のためリフレクションで設定）
            typeof(PictureBox)
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(pictureBox1, true);

            // レイアウト確定後のサイズ変更時にも再描画する
            pictureBox1.Resize += (_, _) => pictureBox1.Invalidate();
        }

        // 画像を完全にメモリへ読み込む（遅延デコードによる初回描画の乱れ・ファイルロックを防ぐ）
        private static Bitmap LoadBitmap(string path)
        {
            using var src = new Bitmap(path);
            return new Bitmap(src);
        }

        // ----------------------------------------------------------------
        // DataGridView セットアップ（NO / X / Y / W / H / 削除ボタン）
        // ----------------------------------------------------------------
        private void SetupDataGridView()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "NO", Name = "colNo", FillWeight = 1, ReadOnly = true });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "X",  Name = "colX",  FillWeight = 1, ReadOnly = true });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Y",  Name = "colY",  FillWeight = 1, ReadOnly = true });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "W",  Name = "colW",  FillWeight = 1, ReadOnly = true });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "H",  Name = "colH",  FillWeight = 1, ReadOnly = true });
            dataGridView1.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "削除",
                Name = "colDelete",
                Text = "削除",
                UseColumnTextForButtonValue = true,
                FillWeight = 1
            });

            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        // ----------------------------------------------------------------
        // カメラリスト読み込み（setting-camera.txt → ListBox）
        // ----------------------------------------------------------------
        private void LoadCameraList()
        {
            _cameras.Clear();
            listBox1.Items.Clear();

            if (!File.Exists(CameraSettingFilePath)) return;

            foreach (var line in File.ReadAllLines(CameraSettingFilePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split('=');
                if (parts.Length != 2) continue;

                var ip    = parts[0].Trim();
                var label = parts[1].Trim();

                _cameras.Add(new CameraInfo(ip, label));
                listBox1.Items.Add(label);
            }
        }

        // ----------------------------------------------------------------
        // カメラ選択変更
        // ----------------------------------------------------------------
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;
            OnCameraListSelectionChanged(_cameras[listBox1.SelectedIndex]);
        }

        private void OnCameraListSelectionChanged(CameraInfo camera)
        {
            _selectedCamera = camera;

            // ズームをリセット
            _zoomFactor = 1.0f;
            _viewOffset = PointF.Empty;

            // 平均画像を読み込む
            _currentImage?.Dispose();
            _currentImage = null;

            if (File.Exists(camera.ImagePath))
            {
                _currentImage = LoadBitmap(camera.ImagePath);
            }
            else
            {
                MessageBox.Show($"画像ファイルが見つかりません。\n{camera.ImagePath}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // LED状態をリセット
            _isLedVisible = false;
            _masterRoiList.Clear();
            ledViewButton.Text = "LEDの表示";

            // 選択状態をリセット
            _selectedRoiIndex = -1;

            // 既存の ROI ファイルがあれば読み込む
            var roiFilePath = Path.Combine(RoiSaveFolderPath, $"{camera.Label}_roi.txt");
            _roiList = LoadRoiFromFile(roiFilePath);

            RefreshRoiGrid();
            pictureBox1.Invalidate();
        }

        // ----------------------------------------------------------------
        // PictureBox 描画（Paint イベント）
        //   ・画像本体（手動描画）
        //   ・確定済み BBOX：シアン色、破線
        //   ・LED マスタ BBOX：緑色、破線
        //   ・描画中 BBOX  ：黄色、破線
        // ----------------------------------------------------------------
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_currentImage == null) return;

            var g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            var imageRect = GetImageRect();

            // 画像本体
            g.DrawImage(_currentImage, imageRect);

            // 確定済み BBOX（シアン）+ 番号
            using var cyanPen = new Pen(Color.Cyan, 2)
                { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            using var labelFont = new Font("Arial", 10, FontStyle.Bold);
            for (int i = 0; i < _roiList.Count; i++)
            {
                var roi = _roiList[i];
                var r = ImageToScreen(roi.X, roi.Y, roi.W, roi.H, imageRect);
                g.DrawRectangle(cyanPen, r);
                string lbl = (i + 1).ToString();
                SizeF lsz = g.MeasureString(lbl, labelFont);
                g.DrawString(lbl, labelFont, Brushes.Cyan, r.Right - lsz.Width, r.Top - lsz.Height);
            }

            // LED マスタ BBOX（緑）
            if (_isLedVisible)
            {
                using var greenPen = new Pen(Color.LimeGreen, 2);
                foreach (var roi in _masterRoiList)
                    g.DrawRectangle(greenPen, ImageToScreen(roi.X, roi.Y, roi.W, roi.H, imageRect));
            }

            // 描画中 BBOX（黄）
            if (_isDrawing)
            {
                using var yellowPen = new Pen(Color.Yellow, 2)
                    { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
                g.DrawRectangle(yellowPen, GetDrawRect(_drawStart, _drawCurrent));
            }

            // 選択中 BBOX（ピンク実線 + ID 右上表示）
            if (_selectedRoiIndex >= 0 && _selectedRoiIndex < _roiList.Count)
            {
                var sel = _roiList[_selectedRoiIndex];
                var sr = ImageToScreen(sel.X, sel.Y, sel.W, sel.H, imageRect);
                using var pinkPen = new Pen(Color.HotPink, 2);
                g.DrawRectangle(pinkPen, sr);

                string label = (_selectedRoiIndex + 1).ToString();
                SizeF sz = g.MeasureString(label, labelFont);
                g.DrawString(label, labelFont, Brushes.HotPink, sr.Right - sz.Width, sr.Top - sz.Height);
            }
        }

        // ----------------------------------------------------------------
        // マウスイベント（BBOX 描画：MouseDown > MouseMove > MouseUp）
        // ----------------------------------------------------------------
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _isDrawing = true;
            _drawStart = e.Location;
            _drawCurrent = e.Location;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing) return;
            _drawCurrent = e.Location;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_isDrawing || e.Button != MouseButtons.Left) return;
            _isDrawing = false;
            _drawCurrent = e.Location;

            var imageRect = GetImageRect();
            if (imageRect.IsEmpty) return;

            var roi = ScreenToImageRoi(_drawStart, _drawCurrent, imageRect);
            if (roi.W <= 0 || roi.H <= 0) return;

            _roiList.Add(roi);
            RefreshRoiGrid();
            pictureBox1.Invalidate();
        }

        // ----------------------------------------------------------------
        // マウスホイール（ズーム）
        // ----------------------------------------------------------------
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            // ホイールイベントを受け取るためにフォーカスを移す
            pictureBox1.Focus();
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_currentImage == null) return;

            const float zoomStep = 0.15f;
            float newZoom = e.Delta > 0
                ? _zoomFactor + zoomStep
                : _zoomFactor - zoomStep;

            if (newZoom <= 1.0f)
            {
                // フル表示に戻す
                _zoomFactor = 1.0f;
                _viewOffset = PointF.Empty;
                pictureBox1.Invalidate();
                return;
            }

            // 現在の画像表示領域でのマウス相対位置を保持したままズーム
            var oldRect = GetImageRect();
            if (oldRect.Width <= 0) return;

            float relX = (float)(e.X - oldRect.X) / oldRect.Width;
            float relY = (float)(e.Y - oldRect.Y) / oldRect.Height;

            _zoomFactor = newZoom;

            // fit rect を再計算してオフセットを更新
            float fitScale = Math.Min(
                (float)pictureBox1.Width  / _currentImage.Width,
                (float)pictureBox1.Height / _currentImage.Height);
            int fitW = (int)(_currentImage.Width  * fitScale);
            int fitH = (int)(_currentImage.Height * fitScale);
            int fitX = (pictureBox1.Width  - fitW) / 2;
            int fitY = (pictureBox1.Height - fitH) / 2;

            int newW = (int)(fitW * _zoomFactor);
            int newH = (int)(fitH * _zoomFactor);

            // マウス位置が変わらないよう offset を調整
            _viewOffset = new PointF(
                e.X - relX * newW - fitX,
                e.Y - relY * newH - fitY);

            pictureBox1.Invalidate();
        }

        // ----------------------------------------------------------------
        // ROI ファイル読み込み（共通処理）
        // ----------------------------------------------------------------
        private static List<RoiEntry> LoadRoiFromFile(string filePath)
        {
            var list = new List<RoiEntry>();
            if (!File.Exists(filePath)) return list;

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                if (parts.Length != 4) continue;
                if (int.TryParse(parts[0], out int x) &&
                    int.TryParse(parts[1], out int y) &&
                    int.TryParse(parts[2], out int w) &&
                    int.TryParse(parts[3], out int h))
                {
                    list.Add(new RoiEntry(x, y, w, h));
                }
            }
            return list;
        }

        // ----------------------------------------------------------------
        // ROI グリッド更新（共通処理）
        // ----------------------------------------------------------------
        private void RefreshRoiGrid()
        {
            dataGridView1.SelectionChanged -= dataGridView1_SelectionChanged;
            dataGridView1.Rows.Clear();
            for (int i = 0; i < _roiList.Count; i++)
            {
                var r = _roiList[i];
                dataGridView1.Rows.Add(i + 1, r.X, r.Y, r.W, r.H);
            }
            dataGridView1.ClearSelection();
            _selectedRoiIndex = -1;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        // ----------------------------------------------------------------
        // ROI 削除（DataGridView 削除ボタン）
        // ----------------------------------------------------------------
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != dataGridView1.Columns["colDelete"]!.Index || e.RowIndex < 0) return;
            DeleteROI(e.RowIndex);
        }

        private void dataGridView1_SelectionChanged(object? sender, EventArgs e)
        {
            _selectedRoiIndex = dataGridView1.CurrentRow?.Index ?? -1;
            pictureBox1.Invalidate();
        }

        private void DeleteROI(int index)
        {
            if (index < 0 || index >= _roiList.Count) return;
            _roiList.RemoveAt(index);
            RefreshRoiGrid();
            pictureBox1.Invalidate();
        }

        // ----------------------------------------------------------------
        // 保存ボタン（カメララベル_roi.txt 出力）
        //   フォーマット：X,Y,W,H（1行1ROI）
        // ----------------------------------------------------------------
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (_selectedCamera == null)
            {
                MessageBox.Show("カメラを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SaveROI();
        }

        private void SaveROI()
        {
            Directory.CreateDirectory(RoiSaveFolderPath);
            var filePath = Path.Combine(RoiSaveFolderPath, $"{_selectedCamera!.Label}_roi.txt");

            var lines = _roiList.Select(r => $"{r.X},{r.Y},{r.W},{r.H}");
            File.WriteAllLines(filePath, lines);

            MessageBox.Show($"保存しました。\n{filePath}", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ----------------------------------------------------------------
        // LED 表示ボタン（マスタデータ読込・トグル）
        // ----------------------------------------------------------------
        private void ledViewButton_Click(object sender, EventArgs e)
        {
            if (_selectedCamera == null)
            {
                MessageBox.Show("カメラを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_isLedVisible)
            {
                _masterRoiList = LoadRoiFromFile(_selectedCamera.MasterDataPath);
                _isLedVisible = true;
                ledViewButton.Text = "LED非表示";
            }
            else
            {
                _isLedVisible = false;
                _masterRoiList.Clear();
                ledViewButton.Text = "LEDの表示";
            }

            pictureBox1.Invalidate();
        }

        // ----------------------------------------------------------------
        // リセットボタン（確認ダイアログ → ROI 全削除）
        // ----------------------------------------------------------------
        private void resetButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "リセットしてもよろしいですか？",
                "確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            _roiList.Clear();
            RefreshRoiGrid();
            pictureBox1.Invalidate();
        }

        // ----------------------------------------------------------------
        // ファイル選択ボタン（デバッグ用：任意の画像を PictureBox に表示）
        // ----------------------------------------------------------------
        private void fIleSelectButton_Click(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "画像ファイル|*.jpg;*.jpeg;*.png;*.bmp|すべてのファイル|*.*",
                Title = "画像ファイルを選択"
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;

            _currentImage?.Dispose();
            _currentImage = LoadBitmap(dialog.FileName);
            _zoomFactor = 1.0f;
            _viewOffset = PointF.Empty;
            pictureBox1.Invalidate();
        }

        // ----------------------------------------------------------------
        // 座標変換ユーティリティ
        // ----------------------------------------------------------------

        // ズームとオフセットを考慮した画像表示領域を返す
        private Rectangle GetImageRect()
        {
            if (_currentImage == null) return Rectangle.Empty;

            // ズーム=1.0 時のフィット矩形
            float fitScale = Math.Min(
                (float)pictureBox1.Width  / _currentImage.Width,
                (float)pictureBox1.Height / _currentImage.Height);

            int fitW = (int)(_currentImage.Width  * fitScale);
            int fitH = (int)(_currentImage.Height * fitScale);
            int fitX = (pictureBox1.Width  - fitW) / 2;
            int fitY = (pictureBox1.Height - fitH) / 2;

            // ズーム適用
            int zoomedW = (int)(fitW * _zoomFactor);
            int zoomedH = (int)(fitH * _zoomFactor);

            return new Rectangle(
                fitX + (int)_viewOffset.X,
                fitY + (int)_viewOffset.Y,
                zoomedW,
                zoomedH);
        }

        // ドラッグ中の矩形を正規化して返す（画面座標）
        private static Rectangle GetDrawRect(Point p1, Point p2) =>
            new(Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X),
                Math.Abs(p1.Y - p2.Y));

        // 画面座標 → 画像座標の ROI 変換
        private RoiEntry ScreenToImageRoi(Point p1, Point p2, Rectangle imageRect)
        {
            var img = _currentImage!;
            float scaleX = (float)img.Width  / imageRect.Width;
            float scaleY = (float)img.Height / imageRect.Height;

            int sx = (int)((Math.Min(p1.X, p2.X) - imageRect.X) * scaleX);
            int sy = (int)((Math.Min(p1.Y, p2.Y) - imageRect.Y) * scaleY);
            int sw = (int)(Math.Abs(p1.X - p2.X) * scaleX);
            int sh = (int)(Math.Abs(p1.Y - p2.Y) * scaleY);

            // 画像の範囲内にクランプ
            sx = Math.Clamp(sx, 0, img.Width  - 1);
            sy = Math.Clamp(sy, 0, img.Height - 1);
            sw = Math.Min(sw, img.Width  - sx);
            sh = Math.Min(sh, img.Height - sy);

            return new RoiEntry(sx, sy, sw, sh);
        }

        // 画像座標 → 画面座標の矩形変換
        private Rectangle ImageToScreen(int x, int y, int w, int h, Rectangle imageRect)
        {
            if (_currentImage == null) return Rectangle.Empty;
            float scaleX = (float)imageRect.Width  / _currentImage.Width;
            float scaleY = (float)imageRect.Height / _currentImage.Height;

            return new Rectangle(
                imageRect.X + (int)(x * scaleX),
                imageRect.Y + (int)(y * scaleY),
                (int)(w * scaleX),
                (int)(h * scaleY));
        }

        // tableLayoutPanel の Paint イベント（空実装）
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e) { }
    }
}
