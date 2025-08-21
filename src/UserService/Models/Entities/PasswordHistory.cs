using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models.Entities;

[Table("password_history", Schema = "users")]
public class PasswordHistory : BaseEntity
{
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("changed_at")]
    public DateTime ChangedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual NguoiDung NguoiDung { get; set; }
}