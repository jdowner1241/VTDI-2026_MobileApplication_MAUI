namespace Mystic_ToDo_MAUI_.Resources.SharedResources.SharedColor.Themes;

public partial class DarkTheme : ResourceDictionary
{
	public DarkTheme()
	{
		InitializeComponent();

        // Colors (raw tokens) only
        this["App_PrimaryColor"] = Color.FromArgb(BaseColors.blue500);
        this["App_SecondaryColor"] = Color.FromArgb(BaseColors.teal200);
        this["App_TertiaryColor"] = Color.FromArgb(BaseColors.blue300);
        this["App_BackgroundColor"] = Color.FromArgb(BaseColors.black500);
        this["App_SurfaceColor"] = Color.FromArgb(BaseColors.gray400);
        this["App_TextColor"] = Color.FromArgb(BaseColors.white900);
        this["App_TextHeaderColor"] = Color.FromArgb(BaseColors.purple300);
        this["App_TextSubHeaderColor"] = Color.FromArgb(BaseColors.blue300);
        this["App_MuteTextColor"] = Color.FromArgb(BaseColors.gray400);
        this["App_InverseTextColor"] = Color.FromArgb(BaseColors.black900);

        // States
        this["App_ErrorColor"] = Color.FromArgb(BaseColors.red500);
        this["App_WarningColor"] = Color.FromArgb(BaseColors.yellow500);
        this["App_InfoColor"] = Color.FromArgb(BaseColors.blue500);
        this["App_SuccessColor"] = Color.FromArgb(BaseColors.green500);
        this["App_DisabledColor"] = Color.FromArgb(BaseColors.gray500);
        this["App_SelectedColor"] = Color.FromArgb(BaseColors.blue600);
        this["App_UnselectedColor"] = Color.FromArgb(BaseColors.gray500);

        // Effect Colors (colors only)
        this["App_GlowColor"] = Color.FromArgb(BaseColors.purple300);
        this["App_ShadowColor"] = Color.FromArgb(BaseColors.gray200);

        // Controls (colors)
        this["App_ControlBackgroundColor"] = Color.FromArgb(BaseColors.blue300);
        this["App_ControlTextColor"] = Color.FromArgb(BaseColors.black500);
        this["App_PlaceholderTextColor"] = Color.FromArgb(BaseColors.gray600);
        this["App_DividerColor"] = Color.FromArgb(BaseColors.purple600);
        this["App_NavigationBarColor"] = Color.FromArgb(BaseColors.purple700);
        this["App_NavigationTextColor"] = Color.FromArgb(BaseColors.black700);
        this["App_TabBarBackgroundColor"] = Color.FromArgb(BaseColors.gray600);
        this["App_TabBarTextColor"] = Color.FromArgb(BaseColors.black400);

    }
}