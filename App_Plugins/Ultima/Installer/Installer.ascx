<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Installer.ascx.cs" Inherits="App_Plugins_Ultima_Installer_Installer" %>

<asp:Panel runat="server" ID="InstallerInfoPanel">

	<asp:Panel runat="server" id="StepPanel">
		<asp:Label runat="server" ID="InstallerLabel"></asp:Label><br/>
		
		<asp:Button runat="server" ID="CompleteInstallationButton" OnClick="CompleteInstallationButton_Click" Text="Installation: Step 2"></asp:Button><br/>
	</asp:Panel>
	<asp:Label runat="server" ID="InstallerLabelResult"></asp:Label>
	

</asp:Panel>
