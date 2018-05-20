// C#研究会
// 2018年GW特別企画
// ブロック崩し

using System;
using System.Drawing;
using System.Windows.Forms;

namespace BreakOut
{
    public partial class Form1 : Form
    {
        private int[] trace = new int[256];
        private int tidx = 0;
        private void Trace(int no)
        {
            trace[tidx++] = no;
            trace[tidx] = -1;
            if (tidx >= 256) tidx = 0;
        }

        /// <summary>
        /// スコア
        /// </summary>
        private int score;

        private int timecount;

        /// <summary>
        /// スコア消去用
        /// </summary>
        private int timeClear;

        /// <summary>
        /// ゲーム状態
        /// </summary>
        private int state;

        /// <summary>
        /// ボール
        /// </summary>
        private CBall Ball;

        /// <summary>
        /// パドル
        /// </summary>
        private CPaddle Paddle;

        /// <summary>
        /// ブロック
        /// </summary>
        private CBlock[] Block;

        /// <summary>
        /// ボールの速さ
        /// </summary>
        private int speed;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Form1_Loadイベント処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //ユーザーがサイズを変更できないようにする
            //最大化、最小化はできる
            FormBorderStyle = FormBorderStyle.FixedSingle;

            // ゲーム初期化
            NewGame();

            // 40fps
            timer1.Interval = 25;
            timer1.Start();

            return;
        }

        /// <summary>
        /// ゲーム初期化
        /// </summary>
        private void NewGame()
        {
            // ゲーム状態設定
            state = 1;
            speed = 4;

            // スコア初期化
            score = 0;

            // 表示管理
            timecount = 0;
            timeClear = timecount;

            // ボール初期化
            Ball = new CBall
            {
                X = 298,
                Y = 400,
                SpeedX = speed,
                SpeedY = -speed
            };

            // パドル初期化
            Paddle = new CPaddle
            {
                X = 274,
                Y = 416,
                SpeedX = 0
            };

            // ブロック初期化
            Block = new CBlock[50];
            int x = 0;
            int y = 128;
            for (int idx = 0; idx < 50; idx++)
            {
                Block[idx] = new CBlock
                {
                    X = 12 + x,
                    Y = 32 + y
                };
                x += 60;
                if (x > pictureBox1.Width - 60)
                {
                    x = 0;
                    y -= 32;
                }
            }


            // PictureBoxと同サイズのBitmapオブジェクトを作成
            Bitmap bmp = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);

            //全体を黒で塗りつぶす
            Graphics objGrp = Graphics.FromImage(bmp);
            objGrp.FillRectangle(Brushes.Black, objGrp.VisibleClipBounds);

            // ブロックを描く
            foreach (var item in Block)
            {
                objGrp.FillRectangle(Brushes.LightGreen, item.X, item.Y,
                    item.Width, item.Height);
            }

            // ビットマップをピクチャボックスに設定する
            pictureBox1.Image = bmp;

            // リソースを解放する
            objGrp.Dispose();

            return;
        }

        /// <summary>
        /// pictureBox1_Paintイベント処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            return;
        }

        /// <summary>
        /// タイマイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            Graphics objGrp = Graphics.FromImage(pictureBox1.Image);

            // パドル消去
            objGrp.FillRectangle(Brushes.Black, Paddle.X, Paddle.Y, Paddle.Width, Paddle.Height);

            // ボール消去
            objGrp.FillEllipse(Brushes.Black, Ball.X, Ball.Y, Ball.Width, Ball.Height);

            //フォーム上の座標でマウスポインタの位置を取得する
            //画面座標でマウスポインタの位置を取得する
            System.Drawing.Point sp = System.Windows.Forms.Cursor.Position;
            //画面座標をクライアント座標に変換する
            System.Drawing.Point cp = this.PointToClient(sp);
            //X座標を取得する
            int x = cp.X;

            // マウスの位置へ向かってパドルを動かす
            if (x < Paddle.X)
            {
                Paddle.SpeedX = -8;
            }
            else if (x > Paddle.X + Paddle.Width)
            {
                Paddle.SpeedX = 8;
            }
            else
            {
                Paddle.SpeedX = 0;
            }
            Paddle.Move();
            Ball.Move();

            // 壁跳ね返り判定
            if ((Ball.X <= 0) && (Ball.SpeedX < 0))
            {
                Ball.SpeedX = -Ball.SpeedX;
            }
            if ((Ball.X + Ball.Width >= pictureBox1.Width) && (Ball.SpeedX > 0))
            {
                Ball.SpeedX = -Ball.SpeedX;
            }
            if (Ball.Y <= 0)
            {
                Ball.SpeedY = -Ball.SpeedY;
            }
            if (Ball.Y + Ball.Height >= pictureBox1.Height)
            {
#if DEBUG
                Ball.SpeedY = -Ball.SpeedY;
#else
                Font objFont = new Font("ＭＳ Ｐゴシック", 64);
                objGrp.DrawString("GAME OVER", objFont, Brushes.White, 60, 200);
                objFont.Dispose();
                timer1.Stop();

                // リソースを解放する
                objGrp.Dispose();
                pictureBox1.Refresh(); // PictureBoxを更新（再描画させる）

                state = 0;
#endif
                return;
            }

            // パドル当たり判定

            if (CheckRectangle(Ball.X, Ball.Y, Ball.Width, Ball.Height,
                                Paddle.X, Paddle.Y, Paddle.Width, Paddle.Height))
            {
                Ball.SpeedY = -Ball.SpeedY;
                Ball.Y = Paddle.Y - Ball.Height;
                Ball.SpeedX = (Ball.X - Paddle.X - Paddle.Width / 2 + Ball.Width / 2) / 2;
            }

            // ブロック当たり判定
            int hit = 0;
            foreach (var item in Block)
            {
                // 消えたブロックは判定対象外とする
                if (item.State == false) continue;

                // ブロックにボールが当たったらブロックを消し、ボールは跳ね返る
                if (CheckRectangle(Ball.X, Ball.Y, Ball.Width, Ball.Height,
                                    item.X, item.Y, item.Width, item.Height))
                {
                    hit++;
                    int dx = item.X - Ball.X - item.Width / 2 - Ball.Width;
                    int dy = item.Y - Ball.Y - item.Height / 2 - Ball.Height;
                    if (dy == 0)
                    {
                        //Ball.SpeedY = -Ball.SpeedY;
                    }
                    else if (Math.Abs(dx / dy) > (item.Width / item.Height))
                    {
                        Ball.SpeedX = -Ball.SpeedX;
                    }
                    else
                    {
                        Ball.SpeedY = -Ball.SpeedY;
                    }
                    item.State = false;
                    objGrp.FillRectangle(Brushes.Black, item.X, item.Y,
                        item.Width, item.Height);
                    Trace(3);
                    break;
                }
            }

            // スコアカウント
            if (hit > 0)
            {
                Font objFont = new Font("ＭＳ Ｐゴシック", 16);
                objGrp.DrawString(score.ToString("000#"), objFont, Brushes.Black, 300, 0);
                score += hit;
                objGrp.DrawString(score.ToString("000#"), objFont, Brushes.Yellow, 300, 0);
                objFont.Dispose();

                // ゲームクリア
                if (score == Block.Length)
                {
                    timer1.Stop();
                    state = 0;

                    objFont = new Font("ＭＳ Ｐゴシック", 64);
                    objGrp.DrawString("GAME CLEAR", objFont, Brushes.White, 50, 180);
                    objFont.Dispose();
                    timer1.Stop();

                    // リソースを解放する
                    objGrp.Dispose();
                    pictureBox1.Refresh(); // PictureBoxを更新（再描画させる）
                    return;
                }
            }

            // パドル描画
            objGrp.FillRectangle(Brushes.LightBlue, Paddle.X, Paddle.Y, Paddle.Width, Paddle.Height);
            // ボール描画
            objGrp.FillEllipse(Brushes.White, Ball.X, Ball.Y, Ball.Width, Ball.Height);

            // 文字を描画する
            timecount++;
            if (timecount % 10 == 0)
            {
                Font objFont = new Font("ＭＳ Ｐゴシック", 16);
                objGrp.DrawString(timeClear.ToString("000#"), objFont, Brushes.Black, 0, 0);
                objGrp.DrawString(timecount.ToString("000#"), objFont, Brushes.Yellow, 0, 0);
                objFont.Dispose();
                timeClear = timecount;
            }

            // リソースを解放する
            objGrp.Dispose();

            pictureBox1.Refresh(); // PictureBoxを更新（再描画させる）

            return;
        }

        /// <summary>
        /// マウス左クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (state == 0)
            {
                NewGame();
                timer1.Start();
            }
        }

        /// <summary>
        /// 重なり判定
        /// </summary>
        /// <param name="ax">aの左座標</param>
        /// <param name="ay">aの上座標</param>
        /// <param name="aw">aの幅</param>
        /// <param name="ah">aの高さ</param>
        /// <param name="bx">bの左座標</param>
        /// <param name="by">bの上座標</param>
        /// <param name="bw">bの幅</param>
        /// <param name="bh">bの左座標</param>
        /// <returns></returns>
        private bool CheckRectangle(int ax, int ay, int aw, int ah, int bx, int by, int bw, int bh)
        {
            // aがbの右にある
            if (ax > bx + bw)
            {
                return false;
            }
            // aがbの左にある
            if (bx > ax + aw)
            {
                return false;
            }
            // aがbの下にある
            if (ay > by + bh)
            {
                return false;
            }
            // aがbの上にある
            if (by > ay + ah)
            {
                return false;
            }
            // aとbは重なっている
            return true;
        }
    }
}
