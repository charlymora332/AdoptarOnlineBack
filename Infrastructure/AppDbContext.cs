using Domain.Entities;
using Domain.Entities.Mascotas;
using Microsoft.EntityFrameworkCore;

namespace AdopcionOnline.Infrastructure

{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<CategoriaEdad> CategoriasEdad { get; set; }
        public DbSet<Tamanio> Tamanio { get; set; }
        public DbSet<CategoriaPersonalidad> CategoriaPersonalidades { get; set; }
        public DbSet<MascotaPersonalidad> MascotaPersonalidades { get; set; }
        public DbSet<CategoriaTipoAnimal> CategoriasTipoAnimal { get; set; } // 👈 agregado
        public DbSet<CategoriaGenero> CategoriasGenero { get; set; } // 👈 agregado

        public DbSet<Imagen> Imagenes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔹 Configuración de relaciones
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Mascotas)
                .WithOne(m => m.Usuario)
                .HasForeignKey(m => m.UsuarioId);

            // 🔹 Relación Usuario → Mascotas (uno a muchos)
            modelBuilder.Entity<Mascota>()
                .HasOne(m => m.Usuario)       // cada mascota tiene un usuario
                .WithMany(u => u.Mascotas)    // un usuario tiene muchas mascotas
                .HasForeignKey(m => m.UsuarioId); // FK en Mascota

            modelBuilder.Entity<MascotaPersonalidad>()
    .HasKey(mp => new { mp.MascotaId, mp.CategoriaPersonalidadId });

            modelBuilder.Entity<Mascota>()
                .HasOne(m => m.CategoriaTipoAnimal)
                .WithMany()
                .HasForeignKey(m => m.TipoAnimalId);
            modelBuilder.Entity<MascotaPersonalidad>()
                .HasOne(mp => mp.Mascota)
                .WithMany(m => m.MascotaPersonalidades)
                .HasForeignKey(mp => mp.MascotaId);

            modelBuilder.Entity<MascotaPersonalidad>()
                .HasOne(mp => mp.CategoriaPersonalidad)
                .WithMany(cp => cp.MascotaPersonalidades) // ⚡ indica la colección correcta
                .HasForeignKey(mp => mp.CategoriaPersonalidadId);

            // 🔹 Seed de Categorías de Edad
            modelBuilder.Entity<CategoriaEdad>().HasData(
                new CategoriaEdad { Id = 1, Nombre = "Cachorro" },
                new CategoriaEdad { Id = 2, Nombre = "Joven" },
                new CategoriaEdad { Id = 3, Nombre = "Adulto" }
            );

            modelBuilder.Entity<CategoriaGenero>().HasData(
               new CategoriaGenero { Id = 1, Nombre = "Hembra" },
                 new CategoriaGenero { Id = 2, Nombre = "Macho" }
               //new CategoriaGenero { Id = 3, Nombre = "Adulto" }
               );

            // 🔹 Seed de Personalidades
            modelBuilder.Entity<CategoriaPersonalidad>().HasData(
                    new CategoriaPersonalidad { Id = 1, Nombre = "Juguetón" },
                    new CategoriaPersonalidad { Id = 2, Nombre = "Tranquilo" },
                    new CategoriaPersonalidad { Id = 3, Nombre = "Protector" }
                );

            // 🔹 Seed de Tipo Animal
            modelBuilder.Entity<CategoriaTipoAnimal>().HasData(
                new CategoriaTipoAnimal { Id = 1, Nombre = "Perro" },
                new CategoriaTipoAnimal { Id = 2, Nombre = "Gato" },
                new CategoriaTipoAnimal { Id = 3, Nombre = "Conejo" },
                new CategoriaTipoAnimal { Id = 4, Nombre = "Ave" }
            );
        }
    }
}