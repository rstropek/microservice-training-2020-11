import { MachineMetadata, MachineSetting, MachineSwitch } from "./apiClient/models";

export class MachineConfigurationViewModel {
    //#region Get references to HTML elements
    private profileDiv: HTMLDivElement = <HTMLDivElement>document.getElementById('profile');
    private login: HTMLAnchorElement = <HTMLAnchorElement>document.getElementById('login');
    private logout: HTMLAnchorElement = <HTMLAnchorElement>document.getElementById('logout');
    private profileLoadingIndicator: HTMLDivElement = <HTMLDivElement>document.getElementById('profile-loading-indicator');
    private profileLoadedContent: HTMLDivElement = <HTMLDivElement>document.getElementById('profile-loaded');
    private welcome: HTMLDivElement = <HTMLParagraphElement>document.getElementById('welcome');

    private loadingIndicator: HTMLDivElement = <HTMLDivElement>document.getElementById('loading-indicator');
    private settingsContainer: HTMLDivElement = <HTMLDivElement>document.getElementById('settings');
    private loadedContent: HTMLDivElement = <HTMLDivElement>document.getElementById('loaded-content');
    private machinesDropdown: HTMLSelectElement = <HTMLSelectElement>document.getElementById('machines-dropdown');
    private machineDescription: HTMLInputElement = <HTMLInputElement>document.getElementById('machine-description');
    private settingName: HTMLInputElement = <HTMLInputElement>document.getElementById('setting-name');
    private settingValue: HTMLInputElement = <HTMLInputElement>document.getElementById('setting-value');
    private settingsUpdateButton: HTMLButtonElement = <HTMLButtonElement>document.getElementById('settings-update-btn');
    private machineContainer: HTMLDivElement = <HTMLDivElement>document.getElementById('machine-container');
    private machineImageContainer: HTMLDivElement = <HTMLDivElement>document.getElementById('machine-image-container');
    private machineSensorValue: HTMLInputElement = <HTMLInputElement>document.getElementById('machine-sensor-value');
    private hook: HTMLElement;
    //#endregion

    //#region Internal fields holding state
    private machineList: MachineMetadata[];
    private machineSettings: MachineSetting[];
    private machineSwitches: MachineSwitch[];
    private selectedMachine: MachineMetadata;
    private selectedSetting: MachineSetting;
    //#endregion

    //#region Callbacks
    /** Callback that will be called when the user selects a new machine */
    public selectMachine: (machine: MachineMetadata) => void;

    /** Callback that will be called when the user clicks on a switch */
    public switchClicked: (machineSwitch: MachineSwitch) => void;

    /** Callback that will be called when the user requests to store machine settings */
    public settingsSaveClicked: (machine: MachineMetadata, settings: MachineSetting[]) => void;
    //#endregion

    //#region Properties for setting machine-related data
    /** Set list of available machines */
    public set machines(machines: MachineMetadata[]) {
        // Clear machines dropdown
        while (this.machinesDropdown.options.length > 0) this.machinesDropdown.remove(0);

        // Cleanup existing image
        this.cleanUpImage();

        // Hide settings container. Will become visible if user selects
        // a setting that she wants to edit.
        this.settingsContainer.hidden = true;

        // Add all machines to selection list
        machines.forEach(m => this.addMachineToSelectionList(m));

        // Store machines for later use
        this.machineList = machines;

        if (this.machineList.length > 0) {
            // Select first machine and set metadata
            this.selectedMachine = this.machineList[0];
            this.setMachineMetadata(this.selectedMachine);

            if (this.selectMachine) {
                // Call callback function
                this.selectMachine(this.selectedMachine);
            }
        }

        // Remove loading indicator
        this.loadingIndicator.hidden = true;
        this.loadedContent.hidden = false;
    }


    /** Set list of available settings for currently selected machine */
    public set settings(settings: MachineSetting[]) {
        this.machineSettings = settings;
        settings.forEach(s => this.addSettingToImage(s));
    }

    /** Set list of available switches for currently selected machine */
    public set switches(switches: MachineSwitch[]) {
        this.machineSwitches = switches;
        switches.forEach(s => this.addSwitchToImage(s));
    }

    /** Set current sensor value for currently selected machine */
    public set sensorValue(value: number) {
        this.machineSensorValue.value = value + "";

        // For demo purposes, we animate the "hook" (element in SVG of crane
        // which represents machine 1).
        this.hook = <HTMLElement>document.getElementById('hook');
        if (this.hook) {
            this.hook.setAttribute('x', value + "");
        }
    }

    public set machineImage(svgImage: string) {
        this.machineImageContainer.innerHTML = svgImage;
    }

    public set profile(profileString: (string | null)) {
        const isSignedIn = !!profileString;

        // Remove loading indicator
        this.profileLoadingIndicator.hidden = true;
        this.profileLoadedContent.hidden = false;

        // Display welcome message if user is signed in
        this.welcome.hidden = !isSignedIn;
        this.profileDiv.innerText = profileString ?? '';

        // Hide/show login/logout depending on whether the user is signed in
        this.login.hidden = isSignedIn;
        this.logout.hidden = !isSignedIn;
    }

    constructor() {
        this.machinesDropdown.onchange = ev => this.onSelectedMachineChanged(ev);
        this.settingsUpdateButton.onclick = () => this.onSettingsUpdate();
    }

    /** Handler for machine selection change */
    private onSelectedMachineChanged(ev: Event): void {
        // Get currently selected machine
        const machineId = ((<HTMLSelectElement>ev.target).value);
        if (machineId && this.machineList && this.machineList.length > 0) {
            // Cleanup settings and image
            this.selectedSetting = null;
            this.settingsContainer.hidden = true;
            this.cleanUpImage();

            // Find selected machine in machine list
            this.selectedMachine = this.machineList.find(m => m.id === machineId);

            // Display machine metadata
            this.setMachineMetadata(this.selectedMachine);

            if (this.selectMachine) {
                // Call callback
                this.selectMachine(this.selectedMachine);
            }
        }
    }

    /** Handler for click on machine settings button */
    private onSettingClicked(ev: Event) {
        // Find machine setting in list of settings
        const clickedSetting = this.machineSettings.find(s => s.id == (<HTMLElement>ev.target).id);

        // Store selected setting for later use
        this.selectedSetting = clickedSetting;

        // Display selected setting
        this.settingsContainer.hidden = false;
        this.settingName.value = clickedSetting.name;
        this.settingValue.value = clickedSetting.value.toString();
    }

    /** Handler for click on machine switch */
    private onSwitchClicked(ev: Event) {
        if (this.switchClicked) {
            // Find switch in list of switches
            const clickedSwitch = this.machineSwitches.find(s => s.id == (<HTMLElement>ev.target).id);

            // Call callback
            this.switchClicked(clickedSwitch);
        }
    }

    /** Handler for "Update settings" button */
    private onSettingsUpdate(): void {
        if (this.settingsSaveClicked && this.selectedSetting) {

            // Copy machine settings
            const dataToUpdate = [... this.machineSettings];

            // Write changed setting
            var settingToUpdate = dataToUpdate.find(s => s.id === this.selectedSetting.id);
            settingToUpdate.value = parseFloat(this.settingValue.value);

            // Call callback
            this.settingsSaveClicked(this.selectedMachine, dataToUpdate);
        }
    }

    /** Add selection option to machine list */
    private addMachineToSelectionList(machine: MachineMetadata): void {
        // Add option to machine list
        const node = document.createElement('option');
        node.setAttribute('value', machine.id);
        const text = document.createTextNode(machine.name);
        node.appendChild(text);
        this.machinesDropdown.append(node);
    }

    /** Display machine metadata */
    private setMachineMetadata(m: MachineMetadata) {
        this.machineDescription.value = m.description;
    }

    /** Removes image, switches, and settings indicators */
    private cleanUpImage() {
        Array.from(this.machineContainer.childNodes)
            .filter(c => c !== this.machineImageContainer)
            .forEach(r => this.machineContainer.removeChild(r));
    }


    /** Add setting indicator to image */
    private addSettingToImage(setting: MachineSetting): void {
        const node = document.createElement('div');
        node.setAttribute('style', "cursor: pointer; font-size: 82px; color: red; position: absolute; left:" + setting.positionX + "px; top:" + setting.positionY + "px;");
        node.setAttribute('id', setting.id);
        const text = document.createTextNode("•");
        node.appendChild(text);
        node.onclick = ev => this.onSettingClicked(ev);
        this.machineContainer.appendChild(node);
    }

    /** Add switch to image */
    private addSwitchToImage(switchToAdd: MachineSwitch): void {
        // Add option to machine list
        const node = document.createElement('button');
        node.setAttribute('style', "position: absolute; left:" + switchToAdd.positionX + "px; top:" + switchToAdd.positionY + "px;");
        node.setAttribute('id', switchToAdd.id);
        const text = document.createTextNode(switchToAdd.name);
        node.appendChild(text);
        node.onclick = ev => this.onSwitchClicked(ev);
        this.machineContainer.appendChild(node);
    }
}
