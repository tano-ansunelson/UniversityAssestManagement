using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniversityAssestManagement.Models;
using UniversityAssetsManagement.Models;
//using UniversityAssetsManagement.Models;

namespace UniversityAssestManagement.Data
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }



        public DbSet<College>  Colleges { get; set; }
       
        public DbSet<Location> Locations { get; set; }
        public DbSet<Asset> Assets { get; set; }

        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

        public DbSet<Technician> Technicians { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<College>(e => {

                e.HasKey(c => c.Id);
                e.Property(c => c.Name).IsRequired().HasMaxLength(100);
                e.Property(c => c.Code).IsRequired().HasMaxLength(10);
                e.Property(c => c.Description).HasMaxLength(300);
                e.Property(c => c.ColorClass).HasMaxLength(50);
            
            
            });




           

            modelBuilder.Entity<Location>(e =>
            {
                e.HasKey(l => l.Id);
                e.Property(l => l.Name).IsRequired().HasMaxLength(100);

                e.HasOne(l => l.College)
                 .WithMany(c =>  c.Locations)
                 .HasForeignKey(l => l.CollegeId)
                 .OnDelete(DeleteBehavior.Restrict);

            });


            modelBuilder.Entity<Asset>(e => 
            {
            
               e.HasKey(a => a.Id);
               e.Property(a =>  a.Name).IsRequired().HasMaxLength(100);
               e.Property(a=> a.Type).IsRequired().HasMaxLength(20);
               e.Property(a => a.Status).IsRequired().HasMaxLength(20);
               e.Property(a => a.Value).HasColumnType("decimal(18,2)");

            
                e.HasOne(a => a.College)
                .WithMany(c => c.Assets)
                .HasForeignKey(a => a.CollegeId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(a => a.Location)
                 .WithMany(l => l.Assets)
                 .HasForeignKey(a => a.LocationId)
                 .OnDelete(DeleteBehavior.Restrict);
            
            });

            modelBuilder.Entity<MaintenanceRecord>(e => {
            e.HasKey(m =>  m.Id);

            e.Property(m=> m.IssueDescription).IsRequired().HasMaxLength(200);
            e.Property(m => m.Cost).HasColumnType("decimal(18,2)");
            e.Property(m => m.Status).IsRequired().HasMaxLength(20);


            e.HasOne(m => m.Asset)
                 .WithMany(a => a.MaintenanceRecords)
                 .HasForeignKey(m => m.AssetId)
                 .OnDelete(DeleteBehavior.Cascade);
            
            });
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.College)
                .WithMany()
                .HasForeignKey(u => u.CollegeId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);


            //   SeedData(modelBuilder);
            modelBuilder.Entity<MaintenanceRecord>()
               .HasOne(m => m.Technician)
               .WithMany(t => t.MaintenanceRecords)
               .HasForeignKey(m => m.TechnicianId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);



            modelBuilder.Entity<Technician>(
                entity => { 
                entity.HasKey(t => t.Id);
                    entity.Property(t => t.FullName).IsRequired().HasMaxLength(100);
                    entity.Property(t => t.Email).IsRequired().HasMaxLength(100);
                    entity.Property(t => t.PhoneNumber).HasMaxLength(20);
                    entity.Property(t => t.Specialty).IsRequired().HasMaxLength(50);
                    entity.HasIndex(t => t.Email)
                    .IsUnique();


                }

                );
               


        }

    }
}

//  private void SeedData(ModelBuilder mb)
//        {
//            mb.Entity<College>().HasData(
//               new College { Id = 1, Name = "College of Engineering", Code = "COE", Description = "Engineering and Technology programs", ColorClass = "stripe-indigo" },
//               new College { Id = 2, Name = "College of Science", Code = "COS", Description = "Natural and Physical Sciences", ColorClass = "stripe-green" },
//               new College { Id = 3, Name = "College of Business", Code = "COB", Description = "Business Administration and Management", ColorClass = "stripe-amber" },
//               new College { Id = 4, Name = "College of Health", Code = "COH", Description = "Health Sciences and Medical programs", ColorClass = "stripe-red" },
//               new College { Id = 5, Name = "College of Agriculture", Code = "COA", Description = "Agricultural Sciences and Farming", ColorClass = "stripe-teal" },
//               new College { Id = 6, Name = "College of Arts", Code = "CART", Description = "Liberal Arts and Humanities", ColorClass = "stripe-orange" }
//           );


//            mb.Entity<Location>().HasData(
//               // COE (1)
//               new Location { Id = 1, Name = "AI Lab", CollegeId = 1 },
//               new Location { Id = 2, Name = "Computer Lab 1", CollegeId = 1 },
//               new Location { Id = 3, Name = "Computer Lab 2", CollegeId = 1 },
//               new Location { Id = 4, Name = "Electronics Workshop", CollegeId = 1 },
//               new Location { Id = 5, Name = "Store Room A", CollegeId = 1 },
//               // COS (2)
//               new Location { Id = 6, Name = "Chemistry Lab", CollegeId = 2 },
//               new Location { Id = 7, Name = "Physics Lab", CollegeId = 2 },
//               new Location { Id = 8, Name = "Biology Lab", CollegeId = 2 },
//               new Location { Id = 9, Name = "Lecture Hall 1", CollegeId = 2 },
//               new Location { Id = 10, Name = "Store Room B", CollegeId = 2 },
//               // COB (3)
//               new Location { Id = 11, Name = "Admin Office", CollegeId = 3 },
//               new Location { Id = 12, Name = "Conference Room A", CollegeId = 3 },
//               new Location { Id = 13, Name = "Conference Room B", CollegeId = 3 },
//               new Location { Id = 14, Name = "Lecture Hall 2", CollegeId = 3 },
//               new Location { Id = 15, Name = "Dean Office", CollegeId = 3 },
//               // COH (4)
//               new Location { Id = 16, Name = "Lab Room 101", CollegeId = 4 },
//               new Location { Id = 17, Name = "Lab Room 102", CollegeId = 4 },
//               new Location { Id = 18, Name = "Simulation Center", CollegeId = 4 },
//               new Location { Id = 19, Name = "Clinic Room", CollegeId = 4 },
//               // COA (5)
//               new Location { Id = 20, Name = "Research Lab", CollegeId = 5 },
//               new Location { Id = 21, Name = "Greenhouse", CollegeId = 5 },
//               new Location { Id = 22, Name = "Field Equipment Store", CollegeId = 5 },
//               // CART (6)
//               new Location { Id = 23, Name = "Art Studio", CollegeId = 6 },
//               new Location { Id = 24, Name = "Music Room", CollegeId = 6 },
//               new Location { Id = 25, Name = "Lecture Hall 3", CollegeId = 6 }
//           );


//            mb.Entity<Asset>().HasData(
//              new Asset { Id = 1, Name = "Dell Laptop Latitude 5520", Type = AssetType.Unique, Status = AssetStatus.Active, Value = 1200m, CollegeId = 1, LocationId = 1, CreatedAt = new DateTime(2024, 1, 1) },
//              new Asset { Id = 2, Name = "HP Printer LaserJet Pro", Type = AssetType.Unique, Status = AssetStatus.Active, Value = 450m, CollegeId = 3, LocationId = 11, CreatedAt = new DateTime(2024, 1, 2) },
//              new Asset { Id = 3, Name = "Office Chairs (50 units)", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 5000m, CollegeId = 2, LocationId = 9, CreatedAt = new DateTime(2024, 1, 3) },
//              new Asset { Id = 4, Name = "Laboratory Microscope", Type = AssetType.Unique, Status = AssetStatus.Damaged, Value = 3200m, CollegeId = 4, LocationId = 16, CreatedAt = new DateTime(2024, 1, 4) },
//              new Asset { Id = 5, Name = "Projector Epson EB-X51", Type = AssetType.Unique, Status = AssetStatus.Active, Value = 800m, CollegeId = 6, LocationId = 25, CreatedAt = new DateTime(2024, 1, 5) },
//               new Asset { Id = 6, Name = "Whiteboard Markers (Box)", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 30m, CollegeId = 1, LocationId = 5, CreatedAt = new DateTime(2024, 1, 6) },
//              new Asset { Id = 7, Name = "Desktop Computer HP Elite", Type = AssetType.Unique, Status = AssetStatus.Active, Value = 950m, CollegeId = 2, LocationId = 6, CreatedAt = new DateTime(2024, 1, 7) },
//              new Asset { Id = 8, Name = "Lab Equipment Set", Type = AssetType.Bulk, Status = AssetStatus.UnderMaintenance, Value = 7500m, CollegeId = 5, LocationId = 20, CreatedAt = new DateTime(2024, 1, 8) },
//              new Asset { Id = 9, Name = "Conference Table", Type = AssetType.Unique, Status = AssetStatus.Active, Value = 1100m, CollegeId = 3, LocationId = 12, CreatedAt = new DateTime(2024, 1, 9) },
//               new Asset { Id = 10, Name = "A4 Paper Reams (100 units)", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 120m, CollegeId = 6, LocationId = 23, CreatedAt = new DateTime(2024, 1, 10) },
//              new Asset { Id = 11, Name = "Air Conditioner Unit", Type = AssetType.Unique, Status = AssetStatus.Damaged, Value = 2200m, CollegeId = 1, LocationId = 4, CreatedAt = new DateTime(2024, 1, 11) },
//              new Asset { Id = 12, Name = "Student Desks (200 units)", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 15000m, CollegeId = 1, LocationId = 2, CreatedAt = new DateTime(2024, 1, 12) },

//new Asset { Id = 13, Name = "Lecture Hall Sound System", Type = AssetType.Unique, Status = AssetStatus.Active, Value = 3500m, CollegeId = 2, LocationId = 7, CreatedAt = new DateTime(2024, 1, 13) },

//new Asset { Id = 14, Name = "Smart Interactive Board", Type = AssetType.Unique, Status = AssetStatus.Active, Value = 2800m, CollegeId = 3, LocationId = 13, CreatedAt = new DateTime(2024, 1, 14) },

//new Asset { Id = 15, Name = "Library Books Collection", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 50000m, CollegeId = 4, LocationId = 17, CreatedAt = new DateTime(2024, 1, 15) },

//new Asset { Id = 16, Name = "Science Laboratory Workstations", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 12000m, CollegeId = 5, LocationId = 18, CreatedAt = new DateTime(2024, 1, 16) },

//new Asset { Id = 17, Name = "Generator Set", Type = AssetType.Unique, Status = AssetStatus.Active, Value = 25000m, CollegeId = 6, LocationId = 24, CreatedAt = new DateTime(2024, 1, 17) },

//new Asset { Id = 18, Name = "Photocopier Machine", Type = AssetType.Unique, Status = AssetStatus.UnderMaintenance, Value = 1800m, CollegeId = 1, LocationId = 3, CreatedAt = new DateTime(2024, 1, 18) },

//new Asset { Id = 19, Name = "Laboratory Benches", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 8000m, CollegeId = 4, LocationId = 15, CreatedAt = new DateTime(2024, 1, 19) },

//new Asset { Id = 20, Name = "CCTV Surveillance System", Type = AssetType.Unique, Status = AssetStatus.Active, Value = 6000m, CollegeId = 2, LocationId = 8, CreatedAt = new DateTime(2024, 1, 20) },

//new Asset { Id = 21, Name = "Library Reading Tables", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 4500m, CollegeId = 3, LocationId = 14, CreatedAt = new DateTime(2024, 1, 21) },

//new Asset { Id = 22, Name = "Campus Network Equipment", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 15000m, CollegeId = 5, LocationId = 19, CreatedAt = new DateTime(2024, 1, 22) },

//new Asset { Id = 23, Name = "Faculty Office Furniture Set", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 7000m, CollegeId = 6, LocationId = 21, CreatedAt = new DateTime(2024, 1, 23) },

//new Asset { Id = 24, Name = "Medical Laboratory Analyzer", Type = AssetType.Unique, Status = AssetStatus.Active, Value = 18000m, CollegeId = 4, LocationId = 16, CreatedAt = new DateTime(2024, 1, 24) },

//new Asset { Id = 25, Name = "Lecture Hall Projectors (10 units)", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 9000m, CollegeId = 2, LocationId = 6, CreatedAt = new DateTime(2024, 1, 25) },

//new Asset { Id = 26, Name = "Solar Power Installation", Type = AssetType.Unique, Status = AssetStatus.Active, Value = 40000m, CollegeId = 5, LocationId = 20, CreatedAt = new DateTime(2024, 1, 26) },

//new Asset { Id = 27, Name = "Examination Hall Chairs", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 12000m, CollegeId = 1, LocationId = 5, CreatedAt = new DateTime(2024, 1, 27) },

//new Asset { Id = 28, Name = "Computer Laboratory Equipment", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 30000m, CollegeId = 3, LocationId = 10, CreatedAt = new DateTime(2024, 1, 28) },

//new Asset { Id = 29, Name = "Water Storage System", Type = AssetType.Unique, Status = AssetStatus.Damaged, Value = 8500m, CollegeId = 6, LocationId = 22, CreatedAt = new DateTime(2024, 1, 29) },

//new Asset { Id = 30, Name = "Research Laboratory Equipment", Type = AssetType.Bulk, Status = AssetStatus.Active, Value = 45000m, CollegeId = 5, LocationId = 18, CreatedAt = new DateTime(2024, 1, 30) }
//          );

//            mb.Entity<MaintenanceRecord>().HasData(
//                new MaintenanceRecord { Id = 1, AssetId = 1, IssueDescription = "Screen flickering issue", Cost = 150m, Status = MaintenanceStatus.Fixed, Date = new DateTime(2024, 1, 15) },
//                new MaintenanceRecord { Id = 2, AssetId = 2, IssueDescription = "Paper jam mechanism repair", Cost = 85m, Status = MaintenanceStatus.Fixed, Date = new DateTime(2024, 1, 14) },
//                new MaintenanceRecord { Id = 3, AssetId = 4, IssueDescription = "Lens replacement needed", Cost = 320m, Status = MaintenanceStatus.Pending, Date = new DateTime(2024, 1, 18) },
//                new MaintenanceRecord { Id = 4, AssetId = 5, IssueDescription = "Bulb replacement", Cost = 200m, Status = MaintenanceStatus.Pending, Date = new DateTime(2024, 1, 17) },
//                new MaintenanceRecord { Id = 5, AssetId = 7, IssueDescription = "Hard drive failure", Cost = 180m, Status = MaintenanceStatus.Fixed, Date = new DateTime(2024, 1, 10) },
//                new MaintenanceRecord { Id = 6, AssetId = 11, IssueDescription = "Compressor not working", Cost = 450m, Status = MaintenanceStatus.Pending, Date = new DateTime(2024, 1, 19) },
//                new MaintenanceRecord { Id = 7, AssetId = 8, IssueDescription = "Calibration required", Cost = 95m, Status = MaintenanceStatus.Fixed, Date = new DateTime(2024, 1, 12) },
//                new MaintenanceRecord { Id = 8, AssetId = 9, IssueDescription = "Surface refinishing", Cost = 120m, Status = MaintenanceStatus.Pending, Date = new DateTime(2024, 1, 20) }
//            );












//        }

