using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Component
{
    public class DropDownItemData<T> where T : class
    {
        public Guid Id { get; set; } = new Guid();
        public T Code { get; set; }
        public string Text { get; set; }
    }
}
