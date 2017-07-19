using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCommandPalette.Plugin
{
    public class PluginMeta
    {
        /// <summary>
        /// Given by the Plugin Download Page. Used to link Plugin to the Plugin page.
        /// </summary>
        public uint PluginPageId { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public Image CommandIcon { get; set; }

        public PluginMeta()
        {
            this.PluginPageId = 0;
            this.Author = "Unkown";
            this.Description = string.Empty;
            this.CommandIcon = null;
        }
    }
}