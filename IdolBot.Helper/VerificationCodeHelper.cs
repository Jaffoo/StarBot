using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace IdolBot.Helper
{
    public class VerifyCodeHelper
    {
        /// <summary>
        /// 预置颜色
        /// </summary>
        private static readonly Color[] Colors = [ Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown,
        Color.Brown,Color.DarkBlue];

        /// <summary>
        /// 验证码字符选择池
        /// </summary>
        private static readonly char[] Chars = [ '2','3','4','5','6','8','9',
       'A','B','C','D','E','F','G','H','J','K', 'L','M','N','P','R','S','T','W','X','Y' ];

        /// <summary>
        /// 验证码字符选择池纯数字
        /// </summary>
        private static readonly char[] NumChars = ['0', '1', '2', '3', '4', '5', '6', '8', '9'];

        /// <summary>
        /// 获取验证码字符串
        /// </summary>
        /// <param name="num">几位</param>
        /// <param name="mixture">是否是混合类型</param>
        /// <returns></returns>
        private static string GenCode(int num, bool mixture = true)
        {
            string code = "";
            Random random = new();
            for (int i = 1; i <= num; i++)
            {
                if (mixture)
                    code += Chars[random.Next(Chars.Length)].ToString();
                else
                    code += NumChars[random.Next(NumChars.Length)].ToString();
            }
            return code;
        }

        #region 普通验证码
        public static (string code, byte[] buffer) CommonVerifyCode(int codeLength = 4, int width = 90, int height = 35, int fontSize = 25, bool mixtrue = true)
        {
            var code = GenCode(codeLength, mixtrue);
            using var image = new Image<Rgba32>(width, height);
            var r = new Random();
            // 字体
            var font = SystemFonts.CreateFont(SystemFonts.Families.First().Name, fontSize, FontStyle.Bold);
            image.Mutate(ctx =>
            {
                // 白底背景
                ctx.Fill(Color.White);

                // 画验证码
                for (int i = 0; i < code.Length; i++)
                {
                    ctx.DrawText(code[i].ToString()
                        , font
                        , Colors[r.Next(Colors.Length)]
                        , new PointF(20 * i + 10, r.Next(2, 12)));
                }

                // 画干扰线
                for (int i = 0; i < 6; i++)
                {
                    var pen = new SolidPen(Colors[r.Next(Colors.Length)], 1);
                    var p1 = new PointF(r.Next(width), r.Next(height));
                    var p2 = new PointF(r.Next(width), r.Next(height));

                    ctx.DrawLine(pen, p1, p2);
                }

                // 画噪点
                for (int i = 0; i < 60; i++)
                {
                    var pen = new SolidPen(Colors[r.Next(Colors.Length)], 1);
                    var p1 = new PointF(r.Next(width), r.Next(height));
                    var p2 = new PointF(p1.X + 1f, p1.Y + 1f);

                    ctx.DrawLine(pen, p1, p2);
                }
            });
            using var ms = new MemoryStream();

            //  格式 自定义
            image.SaveAsPng(ms);
            return (code, ms.ToArray());
        }
        #endregion

        #region 滑动验证码

        #endregion

        #region 选择图案验证码

        #endregion
    }
}
