using System;
using Microsoft.EntityFrameworkCore;
using DomainModel.Classes.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Persistence.EF.PostgreSQL.Models
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.ToTable("utenti");

            builder.Property(u => u.id)
             .HasValueGenerator<SequentialGuidValueGenerator>()
             .HasComment("GUID")
             .HasColumnName("id");

            builder.HasKey(u => new { u.username });

            builder
                .Property(u => u.username)
                .ValueGeneratedNever()
                .HasComment("Username is PK")
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("username")
                .HasColumnType("varchar(50)");

            builder
                .Property(u => u.password)
                .IsRequired()
                .HasColumnName("password");

            builder
                .Property(u => u.firstName)
                .HasColumnName("firstname")
                .HasColumnType("varchar(50)");

            builder
                .Property(u => u.lastName)
                .HasColumnName("lastname")
                .HasColumnType("varchar(50)");

            builder.Property(u => u.timestamp)
                .HasColumnName("timestamp")
                .HasDefaultValue("getdate()");
            //.IsConcurrencyToken();

            builder.Property(u => u.timestamp)
                .ValueGeneratedOnAddOrUpdate()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
            
            builder.Property(u => u.active);

        }
    }
}