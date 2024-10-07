using System;
using SqlD.Attributes;

namespace SqlD.Tests.Framework.Models;

[SqlLiteTable("Any_Table_B")]
public class AnyTableB : IAmATestModel
{
    public long Id { get; set; }

    [SqlLiteColumn("Any_Decimal", SqlLiteType.Real, true)]
    public decimal AnyDecimal { get; set; }

    [SqlLiteColumn("Any_Double", SqlLiteType.Real, true)]
    public double AnyDouble { get; set; }

    [SqlLiteColumn("Any_Integer", SqlLiteType.Integer, true)]
    public int AnyInteger { get; set; }

    [SqlLiteColumn("Any_String", SqlLiteType.Text, true)]
    public string AnyString { get; set; }

    [SqlLiteColumn("Any_DateTime", SqlLiteType.Text, true)]
    public DateTime AnyDateTime { get; set; }

    [SqlLiteIgnore] public AnyEnumeration Any { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(AnyDecimal)}: {AnyDecimal}, {nameof(AnyDouble)}: {AnyDouble}, {nameof(AnyInteger)}: {AnyInteger}, {nameof(AnyString)}: {AnyString}, {nameof(AnyDateTime)}: {AnyDateTime}";
    }
}