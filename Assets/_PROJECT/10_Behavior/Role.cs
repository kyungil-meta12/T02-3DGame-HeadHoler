using System;
using Unity.Behavior;

public enum Team
{
	CitizenSide, // 시민, 경찰 등 모두 포함
	EnemySide    // 가드, 보스 등 모두 포함
}

[BlackboardEnum]
public enum Role
{
	Citizen_None,
	Citizen_Police,
	Enemy_None,
	Enemy_Boss
}
