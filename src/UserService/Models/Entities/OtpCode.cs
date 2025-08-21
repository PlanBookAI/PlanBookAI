using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models.Entities;

[Table("otp_codes", Schema = "users")]
public class OtpCode :BaseEntity
{
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("otp_hash")]
    public string OtpHash { get; set; } = string.Empty;

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("is_used")]
    public bool IsUsed { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual NguoiDung NguoiDung { get; set; }
}