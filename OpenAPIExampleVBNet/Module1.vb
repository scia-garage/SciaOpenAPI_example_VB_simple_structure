Module Module1

    Sub Main()
        Console.WriteLine($"Hello!")
        Dim env As SCIA.OpenAPI.Environment

        env = New SCIA.OpenAPI.Environment("c:\Program Files (x86)\SCIA\Engineer19.0\", "C:Temp\SCIATemp", "1.0.0.0")

        Dim openedSE As Boolean
        openedSE = env.RunSCIAEngineer(SCIA.OpenAPI.Environment.GuiMode.ShowWindowShow)
        If (Not openedSE) Then
            Return
        End If
        Console.WriteLine($"SEn opened")
        Dim proj As SCIA.OpenAPI.EsaProject
        proj = env.OpenProject("C:\Users\jbroz\OneDrive - SCIA nv\WORK\SCIA-ENGINEER\TESTING-PROJECTS\QuickTests\OpenAPIEmptyProject.esa")
        'If (proj = ) Then
        ' Return
        ' End If
        Console.WriteLine($"Proj opened")

        Dim comatid As Guid
        comatid = Guid.NewGuid()
        proj.Model.CreateMaterial(New SCIA.OpenAPI.StructureModelDefinition.Material(comatid, "conc", 0, "C30/37"))
        Dim stmatid As Guid
        stmatid = Guid.NewGuid()
        proj.Model.CreateMaterial(New SCIA.OpenAPI.StructureModelDefinition.Material(stmatid, "steel", 1, "S 355"))
        Dim timatid As Guid
        timatid = Guid.NewGuid()
        proj.Model.CreateMaterial(New SCIA.OpenAPI.StructureModelDefinition.Material(timatid, "timber", 2, "D24 (EN 338)"))
        Dim alumatid As Guid
        alumatid = Guid.NewGuid()
        proj.Model.CreateMaterial(New SCIA.OpenAPI.StructureModelDefinition.Material(alumatid, "alu", 3, "EN-AW 6005A (EP/O,ER/B) T6 (0-5)"))
        Console.WriteLine($"Materials created in ADM")

        proj.Model.CreateCrossSection(New SCIA.OpenAPI.StructureModelDefinition.CrossSectionParametric(Guid.NewGuid(), "conc.rect", comatid, 1, {0.2, 0.4}))
        Dim css_steel As Guid
        css_steel = Guid.NewGuid()
        proj.Model.CreateCrossSection(New SCIA.OpenAPI.StructureModelDefinition.CrossSectionManufactured(css_steel, "steel.HEA", stmatid, "HEA260", 1, 0))
        Console.WriteLine($"CSSs created in ADM")

        Dim a, b, c As Double
        a = 6
        b = 8
        c = 3
        Dim n1, n2, n3, n4, n5, n6, n7, n8 As Guid
        n1 = Guid.NewGuid()
        n2 = Guid.NewGuid()
        n3 = Guid.NewGuid()
        n4 = Guid.NewGuid()
        n5 = Guid.NewGuid()
        n6 = Guid.NewGuid()
        n7 = Guid.NewGuid()
        n8 = Guid.NewGuid()
        proj.Model.CreateNode(New SCIA.OpenAPI.StructureModelDefinition.StructNode(n1, "n1", 0, 0, 0))
        proj.Model.CreateNode(New SCIA.OpenAPI.StructureModelDefinition.StructNode(n2, "n2", a, 0, 0))
        proj.Model.CreateNode(New SCIA.OpenAPI.StructureModelDefinition.StructNode(n3, "n3", a, b, 0))
        proj.Model.CreateNode(New SCIA.OpenAPI.StructureModelDefinition.StructNode(n4, "n4", 0, b, 0))
        proj.Model.CreateNode(New SCIA.OpenAPI.StructureModelDefinition.StructNode(n5, "n5", 0, 0, c))
        proj.Model.CreateNode(New SCIA.OpenAPI.StructureModelDefinition.StructNode(n6, "n6", a, 0, c))
        proj.Model.CreateNode(New SCIA.OpenAPI.StructureModelDefinition.StructNode(n7, "n7", a, b, c))
        proj.Model.CreateNode(New SCIA.OpenAPI.StructureModelDefinition.StructNode(n8, "n8", 0, b, c))

        Dim b1, b2, b3, b4 As Guid
        b1 = Guid.NewGuid()
        b2 = Guid.NewGuid()
        b3 = Guid.NewGuid()
        b4 = Guid.NewGuid()
        proj.Model.CreateBeam(New SCIA.OpenAPI.StructureModelDefinition.Beam(b1, "b1", css_steel, {n1, n5}))
        proj.Model.CreateBeam(New SCIA.OpenAPI.StructureModelDefinition.Beam(b2, "b2", css_steel, {n2, n6}))
        proj.Model.CreateBeam(New SCIA.OpenAPI.StructureModelDefinition.Beam(b3, "b3", css_steel, {n3, n7}))
        proj.Model.CreateBeam(New SCIA.OpenAPI.StructureModelDefinition.Beam(b4, "b4", css_steel, {n4, n8}))

        proj.Model.CreatePointSupport(New SCIA.OpenAPI.StructureModelDefinition.PointSupport(Guid.NewGuid(), "Su1", n1))
        proj.Model.CreatePointSupport(New SCIA.OpenAPI.StructureModelDefinition.PointSupport(Guid.NewGuid(), "Su2", n2))
        proj.Model.CreatePointSupport(New SCIA.OpenAPI.StructureModelDefinition.PointSupport(Guid.NewGuid(), "Su3", n3))
        proj.Model.CreatePointSupport(New SCIA.OpenAPI.StructureModelDefinition.PointSupport(Guid.NewGuid(), "Su4", n4))

        Dim s1 As Guid
        s1 = Guid.NewGuid()
        Dim Nodes = New Guid() {n5, n6, n7, n8}
        proj.Model.CreateSlab(New SCIA.OpenAPI.StructureModelDefinition.Slab(s1, "s1", 0, comatid, 0.15, Nodes))
        Dim lg1 As Guid
        lg1 = Guid.NewGuid()
        proj.Model.CreateLoadGroup(New SCIA.OpenAPI.StructureModelDefinition.LoadGroup(lg1, "lg1", 0))
        Dim lc1 As Guid
        lc1 = Guid.NewGuid()
        proj.Model.CreateLoadCase(New SCIA.OpenAPI.StructureModelDefinition.LoadCase(lc1, "lc1", 0, lg1, 1))
        Dim sf1 As Guid
        sf1 = Guid.NewGuid()
        proj.Model.CreateSurfaceLoad(New SCIA.OpenAPI.StructureModelDefinition.SurfaceLoad(sf1, "sf1", -12500, lc1, s1, 2))


        proj.Model.RefreshModel_ToSCIAEngineer()
        Console.WriteLine($"My model sent to SEn")


        'proj.CreateMesh() 'needs dialogue click

        proj.RunCalculation()
        Console.WriteLine($"My model calculate")

        Dim rapi As SCIA.OpenAPI.Results.ResultsAPI
        rapi = proj.Model.InitializeResultsAPI()
        Dim IntFor1Db1 As SCIA.OpenAPI.Results.Result
        Dim keyIntFor1Db1 As New SCIA.OpenAPI.Results.ResultKey With {
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


        Dim Def2Ds1 As SCIA.OpenAPI.Results.Result
        Dim keyDef2Ds1 = New SCIA.OpenAPI.Results.ResultKey With {
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
            If System.Math.Abs(pivot) > System.Math.Abs(maxvalue) Then
                maxvalue = pivot
            End If
        Next
        Console.WriteLine("Maximum deformation on slab:")
        Console.WriteLine(maxvalue)




        Console.WriteLine($"Press key to exit")
        Console.ReadKey()

        proj.CloseProject(SCIA.OpenAPI.SaveMode.SaveChangesNo)
    End Sub

End Module
