﻿// <auto-generated />
using System;
using DevelopmentCardsApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevelopmentCardsApi.Migrations
{
    [DbContext(typeof(GameContext))]
    [Migration("20200419003144_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3");

            modelBuilder.Entity("DevelopmentCardsApi.Models.DevelopmentCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("DevelopmentCards");
                });

            modelBuilder.Entity("DevelopmentCardsApi.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("DevelopmentCardsApi.Models.GameCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("DevelopmentCardId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Played")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("PlayedAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("PulledAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DevelopmentCardId");

                    b.HasIndex("GameId");

                    b.HasIndex("PlayerId");

                    b.ToTable("GameCards");
                });

            modelBuilder.Entity("DevelopmentCardsApi.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("OriginalGameId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Token")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("DevelopmentCardsApi.Models.GameCard", b =>
                {
                    b.HasOne("DevelopmentCardsApi.Models.DevelopmentCard", "DevelopmentCard")
                        .WithMany()
                        .HasForeignKey("DevelopmentCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DevelopmentCardsApi.Models.Game", "Game")
                        .WithMany("GameCards")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DevelopmentCardsApi.Models.Player", "Player")
                        .WithMany("GameCards")
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("DevelopmentCardsApi.Models.Player", b =>
                {
                    b.HasOne("DevelopmentCardsApi.Models.Game", null)
                        .WithMany("Players")
                        .HasForeignKey("GameId");
                });
#pragma warning restore 612, 618
        }
    }
}
