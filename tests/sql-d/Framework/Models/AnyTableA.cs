using System;
using SqlD.Attributes;

namespace SqlD.Tests.Framework.Models;

[SqlLiteTable("Any_Table_A")]
public class AnyTableA : IAmATestModel
{
    public long Id { get; set; }

    [SqlLiteColumn("Any_Decimal", SqlLiteType.Real, true)]
    [SqlLiteIndex("Any_Decimal_Index", SqlLiteIndexType.NonUnique)]
    public decimal AnyDecimal { get; set; }

    [SqlLiteColumn("Any_Double", SqlLiteType.Real, true)]
    [SqlLiteIndex("Any_Double_Index", SqlLiteIndexType.NonUnique)]
    public double AnyDouble { get; set; }

    [SqlLiteColumn("Any_Integer", SqlLiteType.Integer, true)]
    [SqlLiteIndex("Any_Integer_Index", SqlLiteIndexType.NonUnique)]
    public int AnyInteger { get; set; }

    [SqlLiteColumn("Any_String", SqlLiteType.Text, true)]
    [SqlLiteIndex("Any_String_Index", SqlLiteIndexType.NonUnique, "Any_Integer")]
    public string AnyString { get; set; }

    [SqlLiteColumn("Any_DateTime", SqlLiteType.Text, true)]
    [SqlLiteIndex("Any_DateTime_Index", SqlLiteIndexType.NonUnique)]
    public DateTime AnyDateTime { get; set; }

    [SqlLiteIgnore] public AnyEnumeration Any { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(AnyDecimal)}: {AnyDecimal}, {nameof(AnyDouble)}: {AnyDouble}, {nameof(AnyInteger)}: {AnyInteger}, {nameof(AnyString)}: {AnyString}, {nameof(AnyDateTime)}: {AnyDateTime}";
    }
}