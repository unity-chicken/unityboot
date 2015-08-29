# unityboot

Spring Boot makes it easy to create Unity based Applications that you can "just make your game".

### Features
* Services
* Utils
* Editor Scripts

### Services
* Service class: Service root class
* sb: string bundle service with multi-language support
* ...

### Utils
* ...


#### CsvParser Usage
```c#
class UnitService {
    bool LoadUnitMeta() {
        string text = PersistenceUtil.LoadTextResource("UnitMeta");
        CsvParser parser = new CsvParser();
        parser.Parse(text);

        for (int index = 0; index < parser.Count; index++) {
            CsvRow row = parser.GetRow(index);
            UnitMeta meta = new UnitMeta(row);
            unitMeta.Add(meta.unitType, meta);
        }

        return true;
    }
}

class UnitMeta {
    UnitType unitType { get; set; }
    // ...

    public UnitMeta(CsvRow row) {
        this.unitType = row.NextEnum<UnitType>();
        this.model = row.NextEnum<UnitType>();
        this.attack = row.NextFloat();
        this.health = row.NextFloat();
        this.defense = row.NextFloat();
        this.moveSpeed = row.NextFloat();
        this.attackCoolTime = row.NextFloat();
        this.attackRange = row.NextFloat();
    }    
}
```

### Editor Scripts
* Builder: You can make iOS / Android asset bundles

