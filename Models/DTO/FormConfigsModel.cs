using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class FormConfigsModel
    {
        public bool BannerLink { get; set; }
        public bool TncsLink { get; set; }
        public bool PrivacyLink { get; set; }
    }
}
