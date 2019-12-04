Imports System
Imports System.IO
Imports SCIA.OpenAPI
Imports SCIA.OpenAPI.StructureModelDefinition
Imports SCIA.OpenAPI.Results
Imports SCIA.OpenAPI.OpenAPIEnums

Module Module1

    Private Function GetSEnPath()
        Return "c:\WORK\SCIA-ENGINEER\TESTING-VERSIONS\Full_19.1.2010.32_rel_19.1_patch_2_x86\" ' SEn application installation folder, don't forget run "EP_regsvr32 esa.exe" from commandline with Admin rights
    End Function

    Private Function GetSEnTempPath()
        Return "c:\WORK\SCIA-ENGINEER\TESTING-VERSIONS\Full_19.1.2010.32_rel_19.1_patch_2_x86\Temp\" ' Must be SEn application temp path, run SEn and go to menu: Setup -> Options -> Directories -> Temporary files
    End Function

    Private Function GetThisAppLogPath()
        Return "c:\TEMP\OpenAPI\MyLogsTemp" ' Folder for storing of log files for this console application
    End Function

    Private Function GetSEnProjectFilePath()
        Return "C:\WORK\SourceCodes\OpenAPIExampleVBNet\res\OpenAPIEmptyProject.esa" ' Project which will be used by SEn
    End Function

    Private Function ResolveAssemblies(sender As Object, e As System.ResolveEventArgs) As Reflection.Assembly
        ' Function which is needed for coorect load of SEn assemblies, see the line "AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf ResolveAssemblies"
        Dim dllName = e.Name.Substring(0, e.Name.IndexOf(",")) + ".dll"
        Dim dllFullPath = Path.Combine(GetSEnPath(), dllName)
        If Not System.IO.File.Exists(dllFullPath) Then
            dllFullPath = Path.Combine(GetSEnPath(), "OpenAPI_dll", dllName)
            If Not System.IO.File.Exists(dllFullPath) Then
                Return Nothing
            End If
        End If
        Return Reflection.Assembly.LoadFrom(dllFullPath)
    End Function

    <STAThread()> Private Function CreateModel()
        ' Standalone function where model is created and calculated, please note the "<STAThread()>" in declaration of the function
        Using env As New SCIA.OpenAPI.Environment(GetSEnPath(), GetThisAppLogPath(), "1.0.0.0")

            Dim openedSE As Boolean
            openedSE = env.RunSCIAEngineer(SCIA.OpenAPI.Environment.GuiMode.ShowWindowShow)
            If (Not openedSE) Then
                Return Nothing
            End If
            Console.WriteLine($"SEn opened")
            Dim CurDir As String = My.Application.Info.DirectoryPath
            Dim pathTemplate As String = GetSEnProjectFilePath()
            Dim proj As EsaProject
            proj = env.OpenProject(pathTemplate) 'path to the empty project
            'If (proj = ) Then
            ' Return
            ' End If
            Console.WriteLine($"Proj opened")

            Dim comatid As Guid
            comatid = Guid.NewGuid()
            Console.WriteLine($"Concrete grade: ")
            Dim conMatGrade As String
            conMatGrade = Console.ReadLine()
            proj.Model.CreateMaterial(New Material(comatid, "conc", 0, conMatGrade))
            Dim stmatid As Guid
            stmatid = Guid.NewGuid()
            Console.WriteLine($"Steel grade: ")
            Dim stMatGrade As String
            stMatGrade = Console.ReadLine()
            proj.Model.CreateMaterial(New Material(stmatid, "steel", 1, stMatGrade))
            Dim css_steel As Guid
            css_steel = Guid.NewGuid()
            Console.WriteLine($"Steel profile: ")
            Dim stCss As String
            stCss = Console.ReadLine()
            proj.Model.CreateCrossSection(New CrossSectionManufactured(css_steel, "steel.HEA", stmatid, stCss, 1, 0))
            Console.WriteLine($"CSSs created in ADM")

            Dim a, b, c As Double
            Console.WriteLine($"a: ")
            a = Double.Parse(Console.ReadLine())
            Console.WriteLine($"b: ")
            b = Double.Parse(Console.ReadLine())
            Console.WriteLine($"c: ")
            c = Double.Parse(Console.ReadLine())
            Dim n1, n2, n3, n4, n5, n6, n7, n8 As Guid
            n1 = Guid.NewGuid()
            n2 = Guid.NewGuid()
            n3 = Guid.NewGuid()
            n4 = Guid.NewGuid()
            n5 = Guid.NewGuid()
            n6 = Guid.NewGuid()
            n7 = Guid.NewGuid()
            n8 = Guid.NewGuid()
            proj.Model.CreateNode(New StructNode(n1, "n1", 0, 0, 0))
            proj.Model.CreateNode(New StructNode(n2, "n2", a, 0, 0))
            proj.Model.CreateNode(New StructNode(n3, "n3", a, b, 0))
            proj.Model.CreateNode(New StructNode(n4, "n4", 0, b, 0))
            proj.Model.CreateNode(New StructNode(n5, "n5", 0, 0, c))
            proj.Model.CreateNode(New StructNode(n6, "n6", a, 0, c))
            proj.Model.CreateNode(New StructNode(n7, "n7", a, b, c))
            proj.Model.CreateNode(New StructNode(n8, "n8", 0, b, c))

            Dim b1, b2, b3, b4 As Guid
            b1 = Guid.NewGuid()
            b2 = Guid.NewGuid()
            b3 = Guid.NewGuid()
            b4 = Guid.NewGuid()
            proj.Model.CreateBeam(New Beam(b1, "b1", css_steel, {n1, n5}))
            proj.Model.CreateBeam(New Beam(b2, "b2", css_steel, {n2, n6}))
            proj.Model.CreateBeam(New Beam(b3, "b3", css_steel, {n3, n7}))
            proj.Model.CreateBeam(New Beam(b4, "b4", css_steel, {n4, n8}))

            Dim Su1 As PointSupport
            Su1 = New PointSupport(Guid.NewGuid(), "Su1", n1) With {
                .ConstraintRx = eConstraintType.Free,
                .ConstraintRy = eConstraintType.Free,
                .ConstraintRz = eConstraintType.Free
            }
            proj.Model.CreatePointSupport(Su1)
            proj.Model.CreatePointSupport(New PointSupport(Guid.NewGuid(), "Su2", n2))
            proj.Model.CreatePointSupport(New PointSupport(Guid.NewGuid(), "Su3", n3))
            proj.Model.CreatePointSupport(New PointSupport(Guid.NewGuid(), "Su4", n4))

            Dim s1 As Guid
            s1 = Guid.NewGuid()
            Dim Nodes = New Guid() {n5, n6, n7, n8}
            Dim thickness As Double
            Console.WriteLine($"thickness of slab: ")
            thickness = Double.Parse(Console.ReadLine())
            proj.Model.CreateSlab(New Slab(s1, "s1", 0, comatid, thickness, Nodes))
            Dim lg1 As Guid
            lg1 = Guid.NewGuid()
            proj.Model.CreateLoadGroup(New LoadGroup(lg1, "lg1", 0))
            Dim lc1 As Guid
            lc1 = Guid.NewGuid()
            proj.Model.CreateLoadCase(New LoadCase(lc1, "lc1", 0, lg1, 1))
            'Combination
            Dim combinationItems As CombinationItem() = {New CombinationItem(lc1, 1.5)}
            Dim C1 As Combination
            C1 = New Combination(Guid.NewGuid(), "C1", combinationItems) With
            {
                .Category = eLoadCaseCombinationCategory.AccordingNationalStandard,
                .NationalStandard = eLoadCaseCombinationStandard.EnUlsSetC
            }
            proj.Model.CreateCombination(C1)

            Dim sf1 As Guid
            sf1 = Guid.NewGuid()
            Dim loadValue As Double
            Console.WriteLine($"Value of surface load on slab: ")
            loadValue = Double.Parse(Console.ReadLine())
            proj.Model.CreateSurfaceLoad(New SurfaceLoad(sf1, "sf1", loadValue, lc1, s1, 2))
            ' LINE SUPPORT
            Dim lSupport As LineSupport
            lSupport = New LineSupport(Guid.NewGuid(), "lineSupport", b1) With {
                .Member = b1,
                .ConstraintRx = eConstraintType.Free,
                .ConstraintRy = eConstraintType.Free,
                .ConstraintRz = eConstraintType.Free
            }
            proj.Model.CreateLineSupport(lSupport)
            ' line load
            Dim lload As LineLoadOnBeam
            lload = New LineLoadOnBeam(Guid.NewGuid(), "lineLoad") With {
                .Member = b1,
                .LoadCase = lc1,
                .Value1 = -12500,
                .Value2 = -12500,
                .Direction = eDirection.Z
             }
            proj.Model.CreateLineLoad(lload)

            proj.Model.RefreshModel_ToSCIAEngineer()
            Console.WriteLine($"My model sent to SEn")


            proj.RunCalculation()
            Console.WriteLine($"My model calculate")

            Dim rapi As ResultsAPI
            rapi = proj.Model.InitializeResultsAPI()
            Dim IntFor1Db1 As Result
            Dim keyIntFor1Db1 As New ResultKey With {
                .CaseType = Results64Enums.eDsElementType.eDsElementType_LoadCase,
                .CaseId = lc1,
                .EntityType = Results64Enums.eDsElementType.eDsElementType_Beam,
                .EntityName = "b1",
                .Dimension = Results64Enums.eDimension.eDim_1D,
                .ResultType = Results64Enums.eResultType.eFemBeamInnerForces,
                .CoordSystem = Results64Enums.eCoordSystem.eCoordSys_Local
            }


            IntFor1Db1 = rapi.LoadResult(keyIntFor1Db1)
            Console.WriteLine(IntFor1Db1.GetTextOutput())
            Dim IntFor1Db1Combi As Result
            'Results key for internal forces on beam 1 for combination
            Dim keyIntFor1Db1Combi As New ResultKey With {
                 .EntityType = Results64Enums.eDsElementType.eDsElementType_Beam,
                .EntityName = "b1",
                .CaseType = Results64Enums.eDsElementType.eDsElementType_Combination,
                .CaseId = C1.Id,
                .Dimension = Results64Enums.eDimension.eDim_1D,
                .ResultType = Results64Enums.eResultType.eFemBeamInnerForces,
                .CoordSystem = Results64Enums.eCoordSystem.eCoordSys_Local
             }
            'Load 1D results based on results key
            IntFor1Db1Combi = rapi.LoadResult(keyIntFor1Db1Combi)
            'If (IntFor1Db1Combi!= null) Then

            Console.WriteLine(IntFor1Db1Combi.GetTextOutput())
            'End If



            Dim Def2Ds1 As Result
            Dim keyDef2Ds1 = New ResultKey With {
                .CaseType = Results64Enums.eDsElementType.eDsElementType_LoadCase,
                .CaseId = lc1,
                .EntityType = Results64Enums.eDsElementType.eDsElementType_Slab,
                .EntityName = "s1",
                .Dimension = Results64Enums.eDimension.eDim_2D,
                .ResultType = Results64Enums.eResultType.eFemDeformations,
                .CoordSystem = Results64Enums.eCoordSystem.eCoordSys_Local
            }

            Def2Ds1 = rapi.LoadResult(keyDef2Ds1)
            Console.WriteLine(Def2Ds1.GetTextOutput())

            Dim maxvalue, pivot As Double
            maxvalue = 0
            pivot = maxvalue
            For i As Integer = 0 To Def2Ds1.GetMeshElementCount()
                pivot = Def2Ds1.GetValue(2, i)
                If Math.Abs(pivot) > Math.Abs(maxvalue) Then
                    maxvalue = pivot
                End If
            Next
            Console.WriteLine("Maximum deformation on slab:")
            Console.WriteLine(maxvalue)




            Console.WriteLine($"Press key to exit")
            Console.ReadKey()

            proj.CloseProject(SaveMode.SaveChangesNo)
        End Using
        Return Nothing
    End Function

    ' Please note the "<STAThread()>" in declaration of the Sub Main
    <STAThread()> Sub Main()
        ' Function which is responsible for loading of all necessary assemblies used by SCIA.OpenAPI, must be first line in Main method
        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf ResolveAssemblies

        ' SEn temp folder must be empty before start of the app
        If System.IO.Directory.Exists(GetSEnTempPath()) Then
            System.IO.Directory.Delete(GetSEnTempPath(), True)
        End If
        ' Don't use SCIA.OpenAPI directly in Sub Main(), because it doesn't work together with AssemblyResolve in Sub Main
        CreateModel()
    End Sub

End Module
