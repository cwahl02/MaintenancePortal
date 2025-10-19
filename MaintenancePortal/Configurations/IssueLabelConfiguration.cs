using MaintenancePortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenancePortal.Configurations;

public class IssueLabelConfiguration : IEntityTypeConfiguration<IssueLabel>
{
    public void Configure(EntityTypeBuilder<IssueLabel> builder)
    {
        // Composite Primary Key
        // Helps ensure uniqueness of IssueLabel entries
        builder.HasKey(il => new { il.ParentGene, il.SelfGene, il.LabelId });

        // Relationship: IssueLabel → Issue
        builder.HasOne(il => il.Issue)
            .WithMany(i => i.IssueLabels)
            .HasForeignKey(il => new { il.ParentGene, il.SelfGene })
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: IssueLabel → Label
        builder.HasOne(il => il.Label)
            .WithMany(l => l.IssueLabels)
            .HasForeignKey(il => il.LabelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}