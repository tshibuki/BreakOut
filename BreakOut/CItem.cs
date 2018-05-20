using System.Drawing;

namespace BreakOut
{
    /// <summary>
    /// アイテムクラス
    /// </summary>
    class CItem
    {
        /// <summary>
        /// 座標X
        /// </summary>
        private int _x = 0;
        public int X { get => _x; set => _x = value; }

        /// <summary>
        /// 座標Y
        /// </summary>
        private int _y = 0;
        public int Y { get => _y; set => _y = value; }

        /// <summary>
        /// 幅
        /// </summary>
        private int _width = 16;
        public int Width { get => _width; set => _width = value; }

        /// <summary>
        /// 高さ
        /// </summary>
        private int _height = 16;
        public int Height { get => _height; set => _height = value; }
    }

    /// <summary>
    /// ブロッククラス
    /// </summary>
    class CBlock : CItem
    {
        /// <summary>
        /// ブロックの状態
        /// </summary>
        private bool _state;
        public bool State { get => _state; set => _state = value; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CBlock()
        {
            State = true;
            Width = 56;
            Height = 30;
            return;
        }
    }

    /// <summary>
    /// ボールクラス
    /// </summary>
    class CBall : CItem
    {
        /// <summary>
        /// 移動量
        /// </summary>
        private Point _speed = new Point(0, 0);
        public int SpeedX { get => _speed.X; set => _speed.X = value; }
        public int SpeedY { get => _speed.Y; set => _speed.Y = value; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CBall()
        {
            Width = 16;
            Height = 16;
            return;
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <returns></returns>
        public void Move()
        {
            this.X += _speed.X;
            this.Y += _speed.Y;
            return;
        }
    }

    /// <summary>
    /// パドルクラス
    /// </summary>
    class CPaddle : CItem
    {
        /// <summary>
        /// 移動量
        /// </summary>
        private int _speedX = 0;
        public int SpeedX { get => _speedX; set => _speedX = value; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CPaddle()
        {
            Width = 64;
            Height = 16;
            return;
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <returns></returns>
        public void Move()
        {
            this.X += _speedX;
            return;
        }
    }
}
