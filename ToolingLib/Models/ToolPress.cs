using System.Runtime.Serialization;

namespace ToolingLib
{
    public class ToolRecipe
    {
        public ToolPress[] BarConfiguration { get; set; }
    }

    [System.SerializableAttribute()]
    [DataContract(Name = nameof(ToolPress), Namespace = "")]
    public class ToolPress
    {
        #region Public Constructors

        public ToolPress(int Width, double Position)
        {
            this.Width = Width;
            this.Position = Position;
        }

        #endregion Public Constructors

        #region Properties

        [DataMember(Name = "Position", IsRequired = true)]
        public double Position { get; set; }

        [DataMember(Name = "Width", IsRequired = true)]
        public int Width { get; set; }

        private int Height { get; set; }
        private bool InUse { get; set; }

        #endregion Properties
    }
}