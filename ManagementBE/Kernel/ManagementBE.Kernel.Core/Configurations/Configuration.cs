namespace ManagementBE.Kernel.Core.Configurations
{
    public class Configuration
    {
        public virtual ConnectionString ConnectionStrings { get; set; } = new();
        public virtual SecuritySettings SecuritySettings { get; set; } = new();
        public virtual SwaggerSettings SwaggerSettings { get; set; } = new();
        public virtual CacheSettings CacheSettings { get; set; } = new();
        public virtual RequestResponseLogger RequestResponseLogger { get; set; } = new();
        public virtual IdentitySettings IdentitySettings { get; set; } = new();
    }
}
