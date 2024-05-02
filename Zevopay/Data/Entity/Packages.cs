using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zevopay.Data.Entity
{
    public class Packages
    {
        public int PackageId { get; set; }
        public string? PackageName { get; set; }
    }
}
