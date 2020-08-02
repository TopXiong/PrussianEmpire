using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 玩家选择的角色
 * 主要用于场景之间传递信息
 */
static class PlayerSelect
{
    public static int currentSelect = 0;
    public static List<int> selectPlayers = new List<int>();
}

