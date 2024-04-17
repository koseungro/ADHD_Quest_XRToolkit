using System.Collections;
using System.Collections.Generic;

public class StateEnum
{

}

/// <summary>
/// Focus게임에서 맞췄는지 틀렸는지에 대한 Enum
/// </summary>
public enum FocusState
{
    NONE,
    RIGHT,
    /// <summary>
    /// MISS
    /// </summary>
    OE,
    /// <summary>
    /// WRONG
    /// </summary>
    CE
}

public enum FigureState
{
    NONE,
    TRIANGLE,
    RECTANGULAR,
    CIRCLE,
    DIAMOND,
    PENTAGON
}

public enum LevelType
{ 
    None = 0,    
    ROOM_LEVEL0 = 1,
    ROOM_LEVEL1 = 2,
    ROOM_LEVEL2 = 3,
    ROOM_LEVEL3 = 4,
    LIBRARY_LEVEL0 = 5,
    LIBRARY_LEVEL1 = 6,
    LIBRARY_LEVEL2 = 7,
    LIBRARY_LEVEL3 = 8,
    CAFE_LEVEL0 = 9,
    CAFE_LEVEL1 = 10,
    CAFE_LEVEL2 = 11,
    CAFE_LEVEL3 = 12,
    STREET_LEVEL0 = 13,
    STREET_LEVEL1 = 14,
    STREET_LEVEL2 = 15,
    STREET_LEVEL3 = 16
}

public enum VideoType
{ 
    None = -1,
    PARK = 0,
    TOURIST = 1,
    CAFEALONE1 = 2,
    CAFEALONE2 = 3,
    CAFETOGATHER = 4,
    SCHOOL = 5
}

public enum AOIType
{
    NONE = 0,
    AOI1 = 1,
    AOI2 = 2
}

/// <summary>
/// 키보드 상태
/// </summary>
public enum KoreanState
{
    NONE,
    KOR,
    ENG
}
/// <summary>
/// 키보드의 쉬프트 상태
/// </summary>
public enum ShiftState
{
    OFF,
    ON
}