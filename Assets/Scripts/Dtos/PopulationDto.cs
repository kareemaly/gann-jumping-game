using System.Collections.Generic;

[System.Serializable]
public class PopulationDto
{
    public List<CreatureDto> creatures;

    public PopulationDto(List<CreatureDto> creatures)
    {
        this.creatures = creatures;
    }
}
