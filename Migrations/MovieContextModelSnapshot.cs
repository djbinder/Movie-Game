﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using movieGame.Models;

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
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("movieGame.Models.Actor", b =>
                {
                    b.Property<int>("ActorId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActorName");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<List<string>>("ImageURLs");

                    b.Property<int?>("MovieId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("ActorId");

                    b.HasIndex("MovieId");

                    b.ToTable("Actors");
                });

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

            modelBuilder.Entity("movieGame.Models.Game", b =>
                {
                    b.Property<int>("GameId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<long>("DurationOfGame");

                    b.Property<int>("NumberOfTeamsInGame");

                    b.Property<int>("ThisGamesMovieId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("GameId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("movieGame.Models.GameTeamJoin", b =>
                {
                    b.Property<int>("GameTeamJoinId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ClueGameWonAt");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("GameId");

                    b.Property<int>("PointsReceived");

                    b.Property<int>("TeamId");

                    b.Property<int>("ThisTeamWonId");

                    b.Property<TimeSpan>("TotalTimeTakenForGuesses");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<bool>("WinFlag");

                    b.HasKey("GameTeamJoinId");

                    b.HasIndex("GameId");

                    b.HasIndex("TeamId");

                    b.ToTable("GameTeamJoin");
                });

            modelBuilder.Entity("movieGame.Models.Genre", b =>
                {
                    b.Property<int>("GenreId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("GenreName");

                    b.Property<int?>("MovieId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("GenreId");

                    b.HasIndex("MovieId");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("movieGame.Models.Hints", b =>
                {
                    b.Property<int>("HintsId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Director");

                    b.Property<string>("Genre");

                    b.Property<int>("MovieId");

                    b.Property<string>("ReleaseYear");

                    b.HasKey("HintsId");

                    b.HasIndex("MovieId")
                        .IsUnique();

                    b.ToTable("Hints");
                });

            modelBuilder.Entity("movieGame.Models.Movie", b =>
                {
                    b.Property<int>("MovieId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Decade");

                    b.Property<string>("Director");

                    b.Property<string>("Genre");

                    b.Property<string>("Poster");

                    b.Property<DateTime>("Released");

                    b.Property<TimeSpan>("Runtime");

                    b.Property<string>("Title");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("Year");

                    b.Property<string>("imdbId");

                    b.HasKey("MovieId");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("movieGame.Models.MovieActorJoin", b =>
                {
                    b.Property<int>("MovieActorJoinId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActorId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("MovieId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("MovieActorJoinId");

                    b.HasIndex("ActorId");

                    b.HasIndex("MovieId");

                    b.ToTable("MovieActorJoin");
                });

            modelBuilder.Entity("movieGame.Models.MovieGenreJoin", b =>
                {
                    b.Property<int>("MovieGenreJoinId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("GenreId");

                    b.Property<int>("MovieId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("MovieGenreJoinId");

                    b.HasIndex("GenreId");

                    b.HasIndex("MovieId");

                    b.ToTable("MovieGenreJoin");
                });

            modelBuilder.Entity("movieGame.Models.MovieTeamJoin", b =>
                {
                    b.Property<int>("MovieTeamJoinId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ClueGameWonAt");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("MovieId");

                    b.Property<int>("PointsReceived");

                    b.Property<int>("TeamId");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<bool>("WinFlag");

                    b.HasKey("MovieTeamJoinId");

                    b.HasIndex("MovieId");

                    b.HasIndex("TeamId");

                    b.ToTable("MovieTeamJoin");
                });

            modelBuilder.Entity("movieGame.Models.MovieUserJoin", b =>
                {
                    b.Property<int>("MovieUserJoinId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AttemptCount");

                    b.Property<int>("ClueGameWonAt");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("MovieId");

                    b.Property<int>("PointsReceived");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("UserId");

                    b.Property<bool>("WinFlag");

                    b.HasKey("MovieUserJoinId");

                    b.HasIndex("MovieId");

                    b.HasIndex("UserId");

                    b.ToTable("MovieUserJoin");
                });

            modelBuilder.Entity("movieGame.Models.Poster", b =>
                {
                    b.Property<int>("PosterId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int?>("MovieId");

                    b.Property<string>("Size");

                    b.Property<string>("URL");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("PosterId");

                    b.HasIndex("MovieId");

                    b.ToTable("Posters");
                });

            modelBuilder.Entity("movieGame.Models.Team", b =>
                {
                    b.Property<int>("TeamId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CountOfMoviesGuessedCorrectly");

                    b.Property<int>("CountOfMoviesGuessedIncorrectly");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("GamesPlayed");

                    b.Property<int>("NumberOfPlayersOnTeam");

                    b.Property<string>("TeamName");

                    b.Property<int>("TeamPoints");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("TeamId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("movieGame.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Confirm");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("GamesAttempted");

                    b.Property<int>("GamesWon");

                    b.Property<int>("Points");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("UserCoins");

                    b.Property<string>("UserEmail");

                    b.Property<string>("UserFirstName");

                    b.Property<string>("UserLastName");

                    b.Property<string>("UserPassword");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("movieGame.Models.UserTeamJoin", b =>
                {
                    b.Property<int>("UserTeamJoinId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("TeamId");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("UserId");

                    b.HasKey("UserTeamJoinId");

                    b.HasIndex("TeamId");

                    b.HasIndex("UserId");

                    b.ToTable("UserTeamJoin");
                });

            modelBuilder.Entity("movieGame.Models.Actor", b =>
                {
                    b.HasOne("movieGame.Models.Movie")
                        .WithMany("Actors")
                        .HasForeignKey("MovieId");
                });

            modelBuilder.Entity("movieGame.Models.Clue", b =>
                {
                    b.HasOne("movieGame.Models.Movie")
                        .WithMany("Clues")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("movieGame.Models.GameTeamJoin", b =>
                {
                    b.HasOne("movieGame.Models.Game", "Game")
                        .WithMany("GameTeamJoin")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("movieGame.Models.Team", "Team")
                        .WithMany("GameTeamJoin")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("movieGame.Models.Genre", b =>
                {
                    b.HasOne("movieGame.Models.Movie")
                        .WithMany("Genres")
                        .HasForeignKey("MovieId");
                });

            modelBuilder.Entity("movieGame.Models.Hints", b =>
                {
                    b.HasOne("movieGame.Models.Movie")
                        .WithOne("Hints")
                        .HasForeignKey("movieGame.Models.Hints", "MovieId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("movieGame.Models.MovieActorJoin", b =>
                {
                    b.HasOne("movieGame.Models.Actor", "Actor")
                        .WithMany("MovieActorJoin")
                        .HasForeignKey("ActorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("movieGame.Models.Movie", "Movie")
                        .WithMany("MovieActorJoin")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("movieGame.Models.MovieGenreJoin", b =>
                {
                    b.HasOne("movieGame.Models.Genre", "Genre")
                        .WithMany("MovieGenreJoin")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("movieGame.Models.Movie", "Movie")
                        .WithMany("MovieGenreJoin")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("movieGame.Models.MovieTeamJoin", b =>
                {
                    b.HasOne("movieGame.Models.Movie", "Movie")
                        .WithMany("MovieTeamJoin")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("movieGame.Models.Team", "Team")
                        .WithMany("MovieTeamJoin")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("movieGame.Models.MovieUserJoin", b =>
                {
                    b.HasOne("movieGame.Models.Movie", "Movie")
                        .WithMany("MovieUserJoin")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("movieGame.Models.User", "User")
                        .WithMany("MovieUserJoin")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("movieGame.Models.Poster", b =>
                {
                    b.HasOne("movieGame.Models.Movie", "Movie")
                        .WithMany("Posters")
                        .HasForeignKey("MovieId");
                });

            modelBuilder.Entity("movieGame.Models.UserTeamJoin", b =>
                {
                    b.HasOne("movieGame.Models.Team", "Team")
                        .WithMany("UserTeamJoin")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("movieGame.Models.User", "User")
                        .WithMany("UserTeamJoin")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
