using System.Collections.Generic;
using System.Linq;
using System.Xml;

using UnityEngine;
using Verse;

namespace CustomChemfuelExpanded
{
    public class Settings : ModSettings
    {
        public bool deepchemTankIsDisabled = true;
        public int deepCountPerCell = 30;
        public int deepchemTankStorageCapacity = 2700;
        public int chemfuelTankStorageCapacity = 2700;
        public int ticksToExtract = 200;
        public int ticksToRefine = 600;
        public int deepchemConversionRatio = 2;
        public int chemfuelConversionRatio = 1;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref deepchemTankIsDisabled, "deepchemTankIsDisabled");
            Scribe_Values.Look(ref deepCountPerCell, "deepCountPerCell");
            Scribe_Values.Look(ref deepchemTankStorageCapacity, "deepchemTankStorageCapacity");
            Scribe_Values.Look(ref chemfuelTankStorageCapacity, "chemfuelTankStorageCapacity");
            Scribe_Values.Look(ref ticksToExtract, "ticksToExtract");
            Scribe_Values.Look(ref ticksToRefine, "ticksToRefine");
            Scribe_Values.Look(ref deepchemConversionRatio, "deepchemConversionRatio");
            Scribe_Values.Look(ref chemfuelConversionRatio, "chemfuelConversionRatio");
            base.ExposeData();
        }

        public void UseAlternativePreset()
        {
            deepchemTankIsDisabled = true;
            deepCountPerCell = 30;
            deepchemTankStorageCapacity = 2700;
            chemfuelTankStorageCapacity = 2700;
            ticksToExtract = 200;
            ticksToRefine = 300;
            deepchemConversionRatio = 2;
            chemfuelConversionRatio = 1;
        }

        public void UseVanillaChemfuelExpandedPreset()
        {
            deepchemTankIsDisabled = false;
            deepCountPerCell = 300;
            deepchemTankStorageCapacity = 4500;
            chemfuelTankStorageCapacity = 4500;
            ticksToExtract = 60;
            ticksToRefine = 300;
            deepchemConversionRatio = 1;
            chemfuelConversionRatio = 3;
        }
    }

    public class CustomChemfuelExpandedMod : Mod
    {
        readonly Settings settings;

        public CustomChemfuelExpandedMod(ModContentPack content)
            : base(content)
        {
            settings = GetSettings<Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            float columnWidth = (inRect.width - 36f) / 2f;
            float textFieldWidth = columnWidth / 8f;
            float textFieldHeight = 22f;
            float commonGap = 24f;
            Listing_Standard listingStandard = new Listing_Standard { ColumnWidth = columnWidth };

            listingStandard.Begin(inRect);

            // MARK: Section
            Listing_Standard listingStandard2 = listingStandard.BeginSection(67f);
            Rect rect;

            listingStandard2.Label("Restart the game to apply the changes. Balancing presets:");
            listingStandard2.Gap(12f);

            rect = listingStandard2.GetRect(28f);
            rect.xMin = 2f;
            rect.xMax = rect.xMin + 90f;

            if (Widgets.ButtonText(rect, "Alternative"))
            {
                settings.UseAlternativePreset();
            }

            rect.xMin += 90f + 5f;
            rect.xMax = rect.xMin + 84f;

            if (Widgets.ButtonText(rect, "VCE as is"))
            {
                settings.UseVanillaChemfuelExpandedPreset();
            }
            listingStandard2.EndSection(listingStandard2);

            // MARK: Form
            listingStandard.Gap(36f);

            listingStandard.CheckboxLabeled(
                "Disable deepchem tanks?",
                ref settings.deepchemTankIsDisabled
            );
            listingStandard.Label(
                $"If deepchem tanks are disabled, to compensate, pumpjacks will include the cost of a deepchem tank and be able to store one stack of deepchem (150)."
            );
            listingStandard.Gap((float)commonGap);

            listingStandard.Label(
                $"Amount of deepchem available per cell. One discovered deposit will contain {CalculateChemfuelForADeposit()} of chemfuel."
            );
            rect = listingStandard.GetRect(textFieldHeight);
            rect.xMin = 0f;
            rect.xMax = rect.xMin + textFieldWidth;
            string deepCountPerCellBuffer = settings.deepCountPerCell.ToString();
            Widgets.TextFieldNumeric(
                rect,
                ref settings.deepCountPerCell,
                ref deepCountPerCellBuffer,
                0.0f,
                65000.0f
            );
            listingStandard.Gap((float)commonGap);

            listingStandard.Label("Deepchem tank storage capacity.");
            rect = listingStandard.GetRect(textFieldHeight);
            rect.xMin = 0f;
            rect.xMax = rect.xMin + textFieldWidth;
            string deepchemTankStorageCapacityBuffer =
                settings.deepchemTankStorageCapacity.ToString();
            Widgets.TextFieldNumeric(
                rect,
                ref settings.deepchemTankStorageCapacity,
                ref deepchemTankStorageCapacityBuffer,
                0.0f,
                65000.0f
            );
            listingStandard.Gap((float)commonGap);

            listingStandard.Label("Chemfuel tank storage capacity.");
            rect = listingStandard.GetRect(textFieldHeight);
            rect.xMin = 0f;
            rect.xMax = rect.xMin + textFieldWidth;
            string chemfuelTankStorageCapacityBuffer =
                settings.chemfuelTankStorageCapacity.ToString();
            Widgets.TextFieldNumeric(
                rect,
                ref settings.chemfuelTankStorageCapacity,
                ref chemfuelTankStorageCapacityBuffer,
                0.0f,
                65000.0f
            );
            listingStandard.Gap((float)commonGap);

            // MARK: Column
            listingStandard.NewColumn();
            listingStandard.Gap(215f);

            listingStandard.Label(
                "Ticks it takes to extract 1 deepchem. There’re 60 in-game ticks in a real second."
            );
            rect = listingStandard.GetRect(textFieldHeight);
            rect.xMin = columnWidth + 18f;
            rect.xMax = rect.xMin + textFieldWidth;
            string ticksToExtractBuffer = settings.ticksToExtract.ToString();
            Widgets.TextFieldNumeric(
                rect,
                ref settings.ticksToExtract,
                ref ticksToExtractBuffer,
                0.0f,
                65000.0f
            );
            listingStandard.Gap((float)commonGap);

            listingStandard.Label(
                $"Ticks it takes to refine {settings.deepchemConversionRatio} deepchem into {settings.chemfuelConversionRatio} chemfuel."
            );
            rect = listingStandard.GetRect(textFieldHeight);
            rect.xMin = columnWidth + 18f;
            rect.xMax = rect.xMin + textFieldWidth;
            string ticksToRefineBuffer = settings.ticksToRefine.ToString();
            Widgets.TextFieldNumeric(
                rect,
                ref settings.ticksToRefine,
                ref ticksToRefineBuffer,
                0.0f,
                65000.0f
            );
            listingStandard.Gap((float)commonGap);

            listingStandard.Label("Amount of deepchem (left) that refines to chemfuel (right).");
            rect = listingStandard.GetRect(textFieldHeight);
            rect.xMin = columnWidth + 18f;
            rect.xMax = rect.xMin + textFieldWidth;
            string deepchemConversionRatioBuffer = settings.deepchemConversionRatio.ToString();
            Widgets.TextFieldNumeric(
                rect,
                ref settings.deepchemConversionRatio,
                ref deepchemConversionRatioBuffer,
                0.0f,
                65000.0f
            );

            rect.xMin += textFieldWidth + 4f;
            rect.xMax = rect.xMin + textFieldWidth;
            string chemfuelConversionRatioBuffer = settings.chemfuelConversionRatio.ToString();
            Widgets.TextFieldNumeric(
                rect,
                ref settings.chemfuelConversionRatio,
                ref chemfuelConversionRatioBuffer,
                0.0f,
                65000.0f
            );

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Vanilla Chemfuel Expanded (a third-party settings add-on)";
        }

        private string CalculateChemfuelForADeposit()
        {
            return $"{settings.deepCountPerCell * 20 * settings.chemfuelConversionRatio / settings.deepchemConversionRatio}–{settings.deepCountPerCell * 40 * settings.chemfuelConversionRatio / settings.deepchemConversionRatio}";
        }
    }

    public class PatchOperationUseSettings : PatchOperation
    {
        private readonly Settings settings = LoadedModManager
            .GetMod<CustomChemfuelExpandedMod>()
            .GetSettings<Settings>();

        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool patchOperationIsSuccessful = false;
            Dictionary<string, string> xpathsToReplace = new Dictionary<string, string>()
            {
                {
                    "Defs/ThingDef[defName=\"VCHE_Deepchem\"]/deepCountPerCell",
                    $"<deepCountPerCell>{settings.deepCountPerCell}</deepCountPerCell>"
                },
                {
                    "Defs/ThingDef[defName=\"PS_DeepchemTank\"]/comps/li[@Class=\"PipeSystem.CompProperties_ResourceStorage\"]/storageCapacity",
                    $"<storageCapacity>{settings.deepchemTankStorageCapacity}</storageCapacity>"
                },
                {
                    "Defs/ThingDef[defName=\"PS_ChemfuelTank\"]/comps/li[@Class=\"PipeSystem.CompProperties_ResourceStorage\"]/storageCapacity",
                    $"<storageCapacity>{settings.chemfuelTankStorageCapacity}</storageCapacity>"
                },
                {
                    "Defs/ThingDef[defName=\"VCHE_DeepchemPumpjack\"]/comps/li[@Class=\"PipeSystem.CompProperties_DeepExtractor\"]/ticksPerPortion",
                    $"<ticksPerPortion>{settings.ticksToExtract}</ticksPerPortion>"
                },
                {
                    "Defs/ThingDef[defName=\"PS_DeepchemRefinery\"]/comps/li[@Class=\"PipeSystem.CompProperties_ResourceProcessor\"]/results/li[1]/eachTicks",
                    $"<eachTicks>{settings.ticksToRefine}</eachTicks>"
                },
                {
                    "Defs/ThingDef[defName=\"PS_DeepchemRefinery\"]/comps/li[@Class=\"PipeSystem.CompProperties_ResourceProcessor\"]/results/li[1]/countNeeded",
                    $"<countNeeded>{settings.deepchemConversionRatio}</countNeeded>"
                },
                {
                    "Defs/ThingDef[defName=\"PS_DeepchemRefinery\"]/comps/li[@Class=\"PipeSystem.CompProperties_ResourceProcessor\"]/results/li[1]/thingCount",
                    $"<thingCount>{settings.chemfuelConversionRatio}</thingCount>"
                },
                {
                    "Defs/ThingDef[defName=\"PS_DeepchemRefinery\"]/comps/li[@Class=\"PipeSystem.CompProperties_ResourceProcessor\"]/results/li[1]/netCount",
                    $"<netCount>{settings.chemfuelConversionRatio}</netCount>"
                },
            };

            if (settings.deepchemTankIsDisabled)
            {
                xpathsToReplace.Add(
                    "Defs/ThingDef[defName=\"VCHE_DeepchemPumpjack\"]/costList/Steel",
                    $@"
                    <Steel>{
                        int.Parse(xml.SelectSingleNode("Defs/ThingDef[defName=\"VCHE_DeepchemPumpjack\"]/costList/Steel").InnerText) +
                        int.Parse(xml.SelectSingleNode("Defs/ThingDef[defName=\"PS_DeepchemTank\"]/costList/Steel").InnerText)
                    }</Steel>"
                );

                XmlNode[] deepchemTankNode = xml.SelectNodes(
                        "Defs/ThingDef[defName=\"PS_DeepchemTank\"]"
                    )
                    .Cast<XmlNode>()
                    .ToArray();
                foreach (XmlNode xmlNode in deepchemTankNode)
                {
                    patchOperationIsSuccessful = true;
                    xmlNode.ParentNode.RemoveChild(xmlNode);
                }

                patchOperationIsSuccessful = false;
                XmlDocument containerForNewNode = new XmlDocument();
                containerForNewNode.LoadXml(
                    $@"
                    <li Class=""PipeSystem.CompProperties_ResourceStorage"">
                        <pipeNet>VCHE_DeepchemNet</pipeNet>
                        <storageCapacity>150</storageCapacity>
                        <barSize>(0.46, 0.12)</barSize>
                        <margin>0.06</margin>
                        <centerOffset>(-1.31, 0, 0.93)</centerOffset>
                        <extractOptions>
                            <texPath>UI/Gizmos/ExtractDeepchem</texPath>
                            <labelKey>VCHE_ExtractDeepchem</labelKey>
                            <descKey>VCHE_ExtractDeepchemDesc</descKey>
                            <extractAmount>150</extractAmount>
                            <thing>VCHE_Deepchem</thing>
                            <ratio>1</ratio>
                        </extractOptions>
                    </li>"
                );
                XmlNode newNode = containerForNewNode.DocumentElement;

                foreach (
                    object item in xml.SelectNodes(
                        "Defs/ThingDef[defName=\"VCHE_DeepchemPumpjack\"]/comps"
                    )
                )
                {
                    patchOperationIsSuccessful = true;
                    XmlNode xmlNode = item as XmlNode;
                    xmlNode.AppendChild(xmlNode.OwnerDocument.ImportNode(newNode, deep: true));
                }
            }

            foreach (KeyValuePair<string, string> pair in xpathsToReplace)
            {
                patchOperationIsSuccessful = false;

                XmlDocument containerForNewNode = new XmlDocument();
                containerForNewNode.LoadXml(pair.Value);
                XmlNode newNode = containerForNewNode.DocumentElement;

                XmlNode[] array = xml.SelectNodes(pair.Key).Cast<XmlNode>().ToArray();
                foreach (XmlNode xmlNode in array)
                {
                    patchOperationIsSuccessful = true;
                    XmlNode parentNode = xmlNode.ParentNode;
                    parentNode.InsertBefore(
                        parentNode.OwnerDocument.ImportNode(newNode, deep: true),
                        xmlNode
                    );
                    parentNode.RemoveChild(xmlNode);
                }
            }

            return patchOperationIsSuccessful;
        }
    }
}
