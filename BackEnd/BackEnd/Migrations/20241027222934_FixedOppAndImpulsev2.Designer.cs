﻿// <auto-generated />
using System;
using BackEnd.Controllers.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BackEnd.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241027222934_FixedOppAndImpulsev2")]
    partial class FixedOppAndImpulsev2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BackEnd.Models.BackEndModels.FavoritesModel", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("OpportunityId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "OpportunityId");

                    b.HasIndex("OpportunityId");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.ImpulseModel", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("OpportunityId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpireDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(6,2)");

                    b.HasKey("UserId", "OpportunityId");

                    b.HasIndex("OpportunityId")
                        .IsUnique();

                    b.ToTable("Impulses");
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.OpportunityModel", b =>
                {
                    b.Property<int>("OpportunityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OpportunityId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsImpulsed")
                        .HasColumnType("bit");

                    b.Property<int>("Location")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(6,2)");

                    b.Property<float>("Score")
                        .HasColumnType("real");

                    b.Property<int>("Vacancies")
                        .HasMaxLength(30)
                        .HasColumnType("int");

                    b.Property<DateTime>("date")
                        .HasColumnType("datetime2");

                    b.Property<int>("userID")
                        .HasColumnType("int");

                    b.HasKey("OpportunityId");

                    b.HasIndex("userID");

                    b.ToTable("Opportunities");
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.ReservationModel", b =>
                {
                    b.Property<int>("reservationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("reservationID"));

                    b.Property<DateTime>("checkInDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isActive")
                        .HasColumnType("bit");

                    b.Property<int>("numOfPeople")
                        .HasColumnType("int");

                    b.Property<int>("opportunityID")
                        .HasColumnType("int");

                    b.Property<DateTime>("reservationDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2");

                    b.Property<int>("userID")
                        .HasColumnType("int");

                    b.HasKey("reservationID");

                    b.HasIndex("opportunityID");

                    b.HasIndex("userID");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.ReviewModel", b =>
                {
                    b.Property<int>("ReservationId")
                        .HasColumnType("int");

                    b.Property<string>("Desc")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<float>("Rating")
                        .HasColumnType("real");

                    b.HasKey("ReservationId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.UserModel", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CellPhoneNum")
                        .HasMaxLength(9)
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("ExternalId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("HashedPassword")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("RegistrationDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("TokenExpDate")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.FavoritesModel", b =>
                {
                    b.HasOne("BackEnd.Models.BackEndModels.OpportunityModel", "Opportunity")
                        .WithMany("Favorites")
                        .HasForeignKey("OpportunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackEnd.Models.BackEndModels.UserModel", "User")
                        .WithMany("Favorites")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Opportunity");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.ImpulseModel", b =>
                {
                    b.HasOne("BackEnd.Models.BackEndModels.OpportunityModel", "Opportunity")
                        .WithOne("Impulse")
                        .HasForeignKey("BackEnd.Models.BackEndModels.ImpulseModel", "OpportunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackEnd.Models.BackEndModels.UserModel", "User")
                        .WithMany("Impulses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Opportunity");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.OpportunityModel", b =>
                {
                    b.HasOne("BackEnd.Models.BackEndModels.UserModel", "User")
                        .WithMany("Opportunities")
                        .HasForeignKey("userID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.ReservationModel", b =>
                {
                    b.HasOne("BackEnd.Models.BackEndModels.OpportunityModel", "Opportunity")
                        .WithMany("Reservations")
                        .HasForeignKey("opportunityID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackEnd.Models.BackEndModels.UserModel", "User")
                        .WithMany("Reservations")
                        .HasForeignKey("userID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Opportunity");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.ReviewModel", b =>
                {
                    b.HasOne("BackEnd.Models.BackEndModels.ReservationModel", "Reservation")
                        .WithOne("review")
                        .HasForeignKey("BackEnd.Models.BackEndModels.ReviewModel", "ReservationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.OpportunityModel", b =>
                {
                    b.Navigation("Favorites");

                    b.Navigation("Impulse")
                        .IsRequired();

                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.ReservationModel", b =>
                {
                    b.Navigation("review")
                        .IsRequired();
                });

            modelBuilder.Entity("BackEnd.Models.BackEndModels.UserModel", b =>
                {
                    b.Navigation("Favorites");

                    b.Navigation("Impulses");

                    b.Navigation("Opportunities");

                    b.Navigation("Reservations");
                });
#pragma warning restore 612, 618
        }
    }
}