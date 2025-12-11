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
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>()
       .HasKey(rt => rt.Id);

            modelBuilder.Entity<RefreshToken>()
      .HasOne<Usuario>()
      .WithMany(u => u.RefreshTokens)
      .HasForeignKey(rt => rt.UsuarioId);
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

            modelBuilder.Entity<Tamanio>().HasData(
      new Tamanio { Id = 1, TamanioMascota = "Pequeño" },
        new Tamanio { Id = 2, TamanioMascota = "Mediano" },
        new Tamanio { Id = 3, TamanioMascota = "Grande" }

      );

            // 🔹 Seed de Personalidades
            modelBuilder.Entity<CategoriaPersonalidad>().HasData(
                    new CategoriaPersonalidad { Id = 1, Nombre = "Juguetón" },
                    new CategoriaPersonalidad { Id = 2, Nombre = "Protector" },
                    new CategoriaPersonalidad { Id = 3, Nombre = "Bravo" },
                    new CategoriaPersonalidad { Id = 4, Nombre = "Cariñoso" },
                    new CategoriaPersonalidad { Id = 5, Nombre = "Guardián" },
                    new CategoriaPersonalidad { Id = 6, Nombre = "Tranquilo" }
                );

            // 🔹 Seed de Tipo Animal
            modelBuilder.Entity<CategoriaTipoAnimal>().HasData(
                new CategoriaTipoAnimal { Id = 1, Nombre = "Perro" },
                new CategoriaTipoAnimal { Id = 2, Nombre = "Gato" },
                new CategoriaTipoAnimal { Id = 3, Nombre = "Ave" }
            );
        }
    }
}