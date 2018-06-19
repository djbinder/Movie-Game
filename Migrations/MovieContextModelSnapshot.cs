﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using movieGame.Models;
using System;

namespace movieGame.Migrations
{
    [DbContext(typeof(MovieContext))]
    partial class MovieContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("movieGame.Models.Clue", b =>
                {
                    b.Property<int>("ClueId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ClueDifficulty");

                    b.Property<int>("CluePoints");

                    b.Property<string>("ClueText");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("MovieId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("ClueId");

                    b.HasIndex("MovieId");

                    b.ToTable("Clues");
                });

            modelBuilder.Entity("movieGame.Models.Movie", b =>
                {
                    b.Property<int>("MovieId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int?>("PlayerId");

                    b.Property<string>("Title");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("Year");

                    b.HasKey("MovieId");

                    b.HasIndex("PlayerId");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("movieGame.Models.MoviePlayerJoin", b =>
                {
                    b.Property<int>("MoviePlayerJoinId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("MovieId");

                    b.Property<int>("PlayerId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("MoviePlayerJoinId");

                    b.HasIndex("MovieId");

                    b.HasIndex("PlayerId");

                    b.ToTable("MoviePlayerJoin");
                });

            modelBuilder.Entity("movieGame.Models.Player", b =>
                {
                    b.Property<int>("PlayerId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("GamesPlayed");

                    b.Property<string>("PlayerName");

                    b.Property<int>("Points");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("PlayerId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("movieGame.Models.Clue", b =>
                {
                    b.HasOne("movieGame.Models.Movie")
                        .WithMany("Clues")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("movieGame.Models.Movie", b =>
                {
                    b.HasOne("movieGame.Models.Player")
                        .WithMany("Movies")
                        .HasForeignKey("PlayerId");
                });

            modelBuilder.Entity("movieGame.Models.MoviePlayerJoin", b =>
                {
                    b.HasOne("movieGame.Models.Movie", "Movies")
                        .WithMany("MoviePlayerJoin")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("movieGame.Models.Player", "Players")
                        .WithMany("MoviePlayerJoin")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
