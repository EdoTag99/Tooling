using System.Runtime.Serialization;

namespace ToolingLib
{
    public class MagazineTP
    {
        #region Fields


        private MagazineTool[] magazineTools;

        #endregion Fields

        #region Properties

        public MagazineTool[] MagazineTools { get => magazineTools; set => magazineTools = value; }

        #endregion Properties
    }

    [System.SerializableAttribute()]
    [DataContract(Name = nameof(MagazineTool), Namespace = "")]
    public class MagazineTool
    {
        #region Public Constructors

        public MagazineTool(int Count, int Width)
        {
            this.Count = Count;
            this.Width = Width;
        }

        #endregion Public Constructors

        #region Properties

        [DataMember(Name = "Count", IsRequired = true)]
        public int Count { get; set; }

        [DataMember(Name = "Width", IsRequired = true)]
        public int Width { get; set; }

        #endregion Properties
    }
}