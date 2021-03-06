﻿// <auto-generated />
using System;
using AccountOwner.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AccountOwner.ApiServer.Migrations
{
    [DbContext(typeof(RepositoryContext))]
    [Migration("20200131202008_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AccountOwner.Models.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AccountId")
                        .HasColumnType("char(36)");

                    b.Property<string>("AccountType")
                        .IsRequired()
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("AccountOwner.Models.Owner", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OwnerId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Owner");
                });
#pragma warning restore 612, 618
        }
    }
}
