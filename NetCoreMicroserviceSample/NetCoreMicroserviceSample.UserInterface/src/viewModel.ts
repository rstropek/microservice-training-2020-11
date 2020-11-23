export class MachineConfigurationViewModel {
    //#region Get references to HTML elements
    private profileDiv: HTMLDivElement = <HTMLDivElement>document.getElementById('profile');
    private login: HTMLAnchorElement = <HTMLAnchorElement>document.getElementById('login');
    private logout: HTMLAnchorElement = <HTMLAnchorElement>document.getElementById('logout');
    private profileLoadingIndicator: HTMLDivElement = <HTMLDivElement>document.getElementById('profile-loading-indicator');
    private profileLoadedContent: HTMLDivElement = <HTMLDivElement>document.getElementById('profile-loaded');
    private welcome: HTMLDivElement = <HTMLParagraphElement>document.getElementById('welcome');
    //#endregion

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
}
