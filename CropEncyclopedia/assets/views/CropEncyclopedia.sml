<lane orientation="vertical" padding="10">
  <banner background={@Mods/StardewUI/Sprites/BannerBackground} background-border-thickness="48,0" padding="12" text={#title.cropEncyclopedia} />
  <lane orientation="horizontal">
    <!-- Filters -->
    <frame layout="260px 80%" background={@Mods/StardewUI/Sprites/ControlBorder} padding="32,24" >
      <scrollable>
        <lane orientation="vertical">
          <textinput layout="content 50px" text={<>SearchString} padding="-8,16,0,8"/>
          <checkbox label-text={#options.greenhouseLogic} is-checked={<>GreenhouseLogic} />
          <label text={#options.showOnlyIf} padding="0,16,0,8"/>
          <!-- <checkbox label-text="Perfection" is-checked={<>NeededForPerfection} /> -->
          <checkbox label-text={#options.wontDie} is-checked={<>WontDie} />
          <checkbox label-text={#options.giantCrop} is-checked={<>GiantCrop} />
          <label padding="0,16,0,8" text={#options.seasons} />
          <dropdown option-min-width="210" options={:seasonOptions} option-format={:SeasonFormat} selected-option={<>Season} />
          <label padding="0,16,0,8" text={#options.regrowth} />
          <dropdown option-min-width="210" options={:regrowthOptions} option-format={:RegrowthFormat} selected-option={<>Regrowth} />
          <label padding="0,16,0,8" text={#options.type} />
          <dropdown option-min-width="210" options={:categoryOptions} option-format={:CategoryFormat} selected-option={<>Category} />
          <label padding="0,16,0,8" text={#options.fertilizer} />
          <dropdown option-min-width="210" options={:fertilizerOptions} option-format={:FertilizerFormat} selected-option={<>Fertilizer} />
        </lane>
      </scrollable>
    </frame>
    <lane orientation="vertical" horizontal-content-alignment="middle">
      <frame layout="content 80%" background={@Mods/StardewUI/Sprites/ControlBorder} padding="8" >
        <lane orientation="vertical" >
          <!-- Headers -->
          <frame padding="16" background={@Mods/StardewUI/Sprites/ControlBorder} >
            <lane orientation="horizontal">
              <lane focusable="true" orientation="horizontal" vertical-content-alignment="middle" >
                <label bold="true" max-lines="1" layout="240px content" text={#headers.name} click=|sortBy("Name")| />
                <label bold="true" max-lines="1" layout="36px content" margin="8" text="" />
                <label bold="true" max-lines="1" layout="48px content" text={#headers.growthDays} click=|sortBy("Growth days")| />
                <label bold="true" max-lines="1" layout="48px content" text={#headers.regrowthDays} click=|sortBy("Regrowth days")|/>
                <label bold="true" max-lines="1" layout="48px content" text={#headers.harvests} click=|sortBy("Harvests")|/>
                <label bold="true" max-lines="1" layout="100px content" text={#headers.gold}  click=|sortBy("Gold")| />
                <label bold="true" max-lines="1" layout="100px content" text={#headers.goldPerDay} click=|sortBy("Gold/day")| />
                <label bold="true" max-lines="1" layout="94px content" text={#headers.date} click=|sortBy("Harvest day")| />
                <label bold="true" max-lines="1" layout="168px content" text={#headers.seasons}  />
                <!-- <label max-lines="1" layout="48px content" text="Perfection" click=|sortBy("Perfection")|/> -->
              </lane>
            </lane>
          </frame>

          <scrollable>
            <lane orientation="vertical">
              <frame *repeat={<currentPageSeedInfo} padding="16" background={@Mods/StardewUI/Sprites/ControlBorder}>
                <lane focusable="true" orientation="horizontal" vertical-content-alignment="middle">
                  <frame *context={:harvestItem}>
                    <label layout="240px content" text= {:DisplayName} max-lines="1" />
                  </frame>
                  <image layout="36px" margin="8" sprite={:sprite} />
                  <label  layout="48px content" text= {:daysForGrowth} max-lines="1" />
                  <label layout="48px content" text= {:regrowDaysString}  max-lines="1" />
                  <label layout="48px content" text= {:harvests}  max-lines="1" />
                  <label layout="100px content" text= {:price}  max-lines="1" />
                  <label layout="100px content" text= {:goldPerDay}  max-lines="1" />
                  <lane *context={:harvestDateDisplay} vertical-content-alignment="middle" >
                    <image layout="42px 28px"  sprite={:seasonSprite} />
                    <label layout="48px content" text={:day}  max-lines="1" padding="4,1,0,0" />
                  </lane>
                  <frame *repeat={:seasonSprites}>
                    <image layout="42px 28px" sprite={:this} />
                  </frame>
                  <!-- <label layout="48px content" *if={neededForPerfection} text="!"  max-lines="1" /> -->
                  <!-- <label layout="48px content" *!if={neededForPerfection} text=""  max-lines="1" /> -->
                </lane>
              </frame>
            </lane>
          </scrollable>
        </lane>
      </frame>
      <lane orientation="horizontal" horizontal-content-alignment="middle" vertical-content-alignment="middle" margin="0,8">
        <button text={#page.prev} *if={<hasPrevPage} click=|prevPage()| />
        <button text={#page.prev} *!if={<hasPrevPage} hover-background-tint="#808080"  default-background-tint="#808080" />
        <frame background={@Mods/StardewUI/Sprites/ControlBorder}  margin="0,8" padding="10">
          <lane orientation="horizontal" layout="100px content" margin="10,10" horizontal-content-alignment="middle" >
            <label margin="0,0,8,0" text={<PageIndex} />
            <label margin="0,0,8,0" text={#page.of} />
            <label margin="0,0,8,0" text={<pageCount} />
          </lane>
        </frame>
        <button text={#page.next} *if={<hasNextPage} click=|nextPage()|/>
        <button text={#page.next} *!if={<hasNextPage} hover-background-tint="#808080" default-background-tint="#808080"/>
      </lane>
    </lane>
  </lane>
</lane>
