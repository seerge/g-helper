sc config  AsusAppService start= auto
sc config  ASUSLinkNear start= auto
sc config  ASUSLinkRemote start= auto
sc config  ASUSSoftwareManager start= auto
sc config  ASUSSwitch start= auto
sc config  ASUSSystemAnalysis start= auto
sc config  ASUSSystemDiagnosis start= auto
sc config  ArmouryCrateControlInterface start= auto

sc START  AsusAppService
sc START  ASUSLinkNear
sc START  ASUSLinkRemote
sc START  ASUSSoftwareManager
sc START  ASUSSwitch
sc START  ASUSSystemAnalysis
sc START  ASUSSystemDiagnosis
sc START ArmouryCrateControlInterface

set /p asd="Hit enter to finish"
