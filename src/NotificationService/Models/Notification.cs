using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace NotificationService.Models
{
    // Ánh xạ lớp này tới bảng 'notifications.notifications' trong cơ sở dữ liệu.
    [Table("notifications", Schema = "notifications")]
    public class Notification
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("user_id")]
        public string UserId { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("message")]
        public string Message { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("scheduled_at")]
        public DateTime? ScheduledAt { get; set; }

        [Column("template_data", TypeName = "jsonb")]
        public object TemplateData { get; set; }

        [Column("bulk_user_ids")]
        public Guid[] BulkUserIds { get; set; }

        [Column("parent_notification_id")]
        public Guid? ParentNotificationId { get; set; }

        [Column("retry_count")]
        public int RetryCount { get; set; }

        [Column("max_retries")]
        public int MaxRetries { get; set; }

        [Required]
        [Column("status")]
        public string Status { get; set; }
    }
}
