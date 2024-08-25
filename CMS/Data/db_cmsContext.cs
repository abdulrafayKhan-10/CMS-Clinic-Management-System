using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CMS.Models;

namespace CMS.Data
{
    public partial class db_cmsContext : DbContext
    {
        public db_cmsContext()
        {
        }

        public db_cmsContext(DbContextOptions<db_cmsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblAbout> TblAbouts { get; set; } = null!;
        public virtual DbSet<TblAdmin> TblAdmins { get; set; } = null!;
        public virtual DbSet<TblCheckout> TblCheckouts { get; set; } = null!;
        public virtual DbSet<TblClientRegister> TblClientRegisters { get; set; } = null!;
        public virtual DbSet<TblEventBooking> TblEventBookings { get; set; } = null!;
        public virtual DbSet<TblFeedback> TblFeedbacks { get; set; } = null!;
        public virtual DbSet<TblImage> TblImages { get; set; } = null!;
        public virtual DbSet<TblOrder> TblOrders { get; set; } = null!;
        public virtual DbSet<TblProduct> TblProducts { get; set; } = null!;
        public virtual DbSet<TblUpcomingEvent> TblUpcomingEvents { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=db_cms;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblAbout>(entity =>
            {
                entity.ToTable("tbl_about");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Headingfour)
                    .IsUnicode(false)
                    .HasColumnName("headingfour");

                entity.Property(e => e.Headingone)
                    .IsUnicode(false)
                    .HasColumnName("headingone");

                entity.Property(e => e.Headingthree)
                    .IsUnicode(false)
                    .HasColumnName("headingthree");

                entity.Property(e => e.Headingtwo)
                    .IsUnicode(false)
                    .HasColumnName("headingtwo");

                entity.Property(e => e.Imagefour)
                    .IsUnicode(false)
                    .HasColumnName("imagefour");

                entity.Property(e => e.Imageone)
                    .IsUnicode(false)
                    .HasColumnName("imageone");

                entity.Property(e => e.Imagethree)
                    .IsUnicode(false)
                    .HasColumnName("imagethree");

                entity.Property(e => e.Imagetwo)
                    .IsUnicode(false)
                    .HasColumnName("imagetwo");

                entity.Property(e => e.MainTitle)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("main_title");

                entity.Property(e => e.Textfour)
                    .IsUnicode(false)
                    .HasColumnName("textfour");

                entity.Property(e => e.Textone)
                    .IsUnicode(false)
                    .HasColumnName("textone");

                entity.Property(e => e.Textthree)
                    .IsUnicode(false)
                    .HasColumnName("textthree");

                entity.Property(e => e.Texttwo)
                    .IsUnicode(false)
                    .HasColumnName("texttwo");
            });

            modelBuilder.Entity<TblAdmin>(entity =>
            {
                entity.ToTable("tbl_admin");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AdminName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("admin_name");

                entity.Property(e => e.Image)
                    .IsUnicode(false)
                    .HasColumnName("image");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<TblCheckout>(entity =>
            {
                entity.ToTable("tbl_checkout");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.PDes)
                    .IsUnicode(false)
                    .HasColumnName("p_des");

                entity.Property(e => e.PName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("p_name");

                entity.Property(e => e.PPrice).HasColumnName("p_price");

                entity.Property(e => e.PQty).HasColumnName("p_qty");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.TblCheckouts)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_tbl_checkout_tbl_orders");
            });

            modelBuilder.Entity<TblClientRegister>(entity =>
            {
                entity.ToTable("tbl_client_register");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Desc)
                    .IsUnicode(false)
                    .HasColumnName("desc");

                entity.Property(e => e.Email)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Image)
                    .IsUnicode(false)
                    .HasColumnName("image");

                entity.Property(e => e.Name)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .IsUnicode(false)
                    .HasColumnName("password");
            });

            modelBuilder.Entity<TblEventBooking>(entity =>
            {
                entity.ToTable("tbl_event_booking");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.EventId).HasColumnName("event_id");

                entity.Property(e => e.Img)
                    .IsUnicode(false)
                    .HasColumnName("img");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.TblEventBookings)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_tbl_event_booking_tbl_upcoming_event");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblEventBookings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_tbl_event_booking_tbl_event_booking");
            });

            modelBuilder.Entity<TblFeedback>(entity =>
            {
                entity.ToTable("tbl_feedback");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PId).HasColumnName("p_id");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.Text)
                    .IsUnicode(false)
                    .HasColumnName("text");

                entity.Property(e => e.Title)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.PIdNavigation)
                    .WithMany(p => p.TblFeedbacks)
                    .HasForeignKey(d => d.PId)
                    .HasConstraintName("FK_tbl_feedback_tbl_products");
            });

            modelBuilder.Entity<TblImage>(entity =>
            {
                entity.ToTable("tbl_images");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AltTxt)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("alt_txt");

                entity.Property(e => e.ImageName)
                    .IsUnicode(false)
                    .HasColumnName("image_name");

                entity.Property(e => e.PId).HasColumnName("p_id");

                entity.HasOne(d => d.PIdNavigation)
                    .WithMany(p => p.TblImages)
                    .HasForeignKey(d => d.PId)
                    .HasConstraintName("FK_tbl_images_tbl_products");
            });

            modelBuilder.Entity<TblOrder>(entity =>
            {
                entity.ToTable("tbl_orders");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("address");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TotalPurchase).HasColumnName("total_purchase");

                entity.Property(e => e.UId).HasColumnName("u_id");
            });

            modelBuilder.Entity<TblProduct>(entity =>
            {
                entity.ToTable("tbl_products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Category)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("category");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("company_name");

                entity.Property(e => e.Description)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Image)
                    .IsUnicode(false)
                    .HasColumnName("image");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("product_name");

                entity.Property(e => e.Saleprice).HasColumnName("saleprice");

                entity.Property(e => e.Stock).HasColumnName("stock");
            });

            modelBuilder.Entity<TblUpcomingEvent>(entity =>
            {
                entity.ToTable("tbl_upcoming_event");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Banner)
                    .IsUnicode(false)
                    .HasColumnName("banner");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.Details)
                    .IsUnicode(false)
                    .HasColumnName("details");

                entity.Property(e => e.EntryFee).HasColumnName("entry_fee");

                entity.Property(e => e.Faculty)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("faculty");

                entity.Property(e => e.Location)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("location");

                entity.Property(e => e.Time).HasColumnName("time");

                entity.Property(e => e.Topic)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("topic");

                entity.Property(e => e.TotalSeats).HasColumnName("total_seats");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
