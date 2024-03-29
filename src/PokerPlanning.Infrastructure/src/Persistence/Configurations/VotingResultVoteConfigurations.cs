using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokerPlanning.Domain.src.Models.GameAggregate.Entities;

namespace PokerPlanning.Infrastructure.src.Persistence.Configurations;

public class VotingResultVoteConfigurations : IEntityTypeConfiguration<VotingResultVote>
{
    public void Configure(EntityTypeBuilder<VotingResultVote> builder)
    {
        builder.HasOne(vrv => vrv.Vote)
            .WithMany()
            .IsRequired(false)
            .HasForeignKey(vrv => vrv.VoteId);
        builder.HasOne(vrv => vrv.Participant)
            .WithMany()
            .HasForeignKey(vrv => vrv.ParticipantId)
            .IsRequired();
        builder.ToTable("VotingResultVotes");
    }
}
