using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models.DTOs
{
    /// <summary>
    /// Lớp DTO (Data Transfer Object) cho yêu cầu gửi thông báo hàng loạt.
    /// Dùng để truyền dữ liệu từ API endpoint đến service.
    /// </summary>
    public class BulkNotificationRequestDto
    {
        /// <summary>
        /// ID của template thông báo sẽ được sử dụng.
        /// Bắt buộc phải có để xác định nội dung thông báo.
        /// </summary>
        [Required(ErrorMessage = "TemplateId is required.")]
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Danh sách ID người dùng nhận thông báo.
        /// Bắt buộc phải có và không được rỗng.
        /// </summary>
        [Required(ErrorMessage = "UserIds list is required.")]
        [MinLength(1, ErrorMessage = "At least one UserId is required.")]
        public List<Guid> UserIds { get; set; }

        /// <summary>
        /// Dữ liệu biến cho template ở định dạng JSON.
        /// </summary>
        public object Variables { get; set; }
    }
}
