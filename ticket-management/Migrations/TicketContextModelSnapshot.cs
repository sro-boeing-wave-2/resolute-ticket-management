﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ticket_management.Models;

namespace ticket_management.Migrations
{
    [DbContext(typeof(TicketContext))]
    partial class TicketContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ticket_management.Models.Analytics", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Avgresolutiontime");

                    b.Property<double>("Csatscore");

                    b.Property<int>("Customerid");

                    b.Property<DateTime>("Date");

                    b.HasKey("Id");

                    b.ToTable("Analytics");
                });

            modelBuilder.Entity("ticket_management.Models.Comments", b =>
                {
                    b.Property<long>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment");

                    b.Property<long>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<long?>("TicketId");

                    b.Property<long>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("CommentId");

                    b.HasIndex("TicketId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("ticket_management.Models.Ticket", b =>
                {
                    b.Property<long>("TicketId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("Agentid");

                    b.Property<string>("Connectionid");

                    b.Property<long>("CreatedBy");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<long>("Customerid");

                    b.Property<long>("Departmentid");

                    b.Property<string>("Description");

                    b.Property<int>("Feedbackscore");

                    b.Property<string>("Priority");

                    b.Property<long>("Sla");

                    b.Property<string>("Source");

                    b.Property<int>("Status");

                    b.Property<string>("Subject");

                    b.Property<long>("UpdatedBy");

                    b.Property<DateTime>("UpdatedOn");

                    b.Property<long>("Userid");

                    b.HasKey("TicketId");

                    b.ToTable("Ticket");
                });

            modelBuilder.Entity("ticket_management.Models.Comments", b =>
                {
                    b.HasOne("ticket_management.Models.Ticket")
                        .WithMany("Comment")
                        .HasForeignKey("TicketId");
                });
#pragma warning restore 612, 618
        }
    }
}
