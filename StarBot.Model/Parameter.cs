using System.ComponentModel.DataAnnotations;

namespace StarBot.Model
{
    /// <summary>
    /// 分页参数
    /// </summary>
    public class PageModel
    {
        /// <summary>
        /// 页码
        /// </summary>
        [Required]
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 页大小
        /// </summary>
        [Required]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword
        {
            get => _keyword ?? "";
            set => _keyword = value ?? "";
        }

        private string? _keyword;
    }
}
