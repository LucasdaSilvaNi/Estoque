using Sam.Domain.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sam.Domain.Entity;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for SubItemMaterialInfraestructureTest and is intended
    ///to contain all SubItemMaterialInfraestructureTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubItemMaterialInfraestructureTest
    {

        public static void SystemWhiteLinePre(DateTime dataInicio)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("1.0 - Tempo: {0}", DateTime.Now));
        }

        public static void SystemWhiteLinePos(DateTime dataInicio)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("2.0 - Tempo: {0}", DateTime.Now));

            DateTime dataFinal = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("Processamento: {0}", dataFinal - dataInicio));
        }


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Select
        ///</summary>
        [TestMethod()]
        public void SelectTest()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int _id = 28434; 
            SubItemMaterialEntity expected = null; 
            SubItemMaterialEntity actual;

            
            DateTime dataInicio = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("1.0 - Tempo: {0}", DateTime.Now)); 
            actual = target.Select(_id);
            System.Diagnostics.Debug.WriteLine(String.Format("2.0 - Tempo: {0}", DateTime.Now));
            
            DateTime dataFinal = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("Processamento: {0}", dataFinal - dataInicio));

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for ListarSubItemSaldoByAlmox
        ///</summary>
        [TestMethod()]
        public void ListarSubItemSaldoByAlmoxTest()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int almoxarifado = 93; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            DateTime dataInicio = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("1.0 - Tempo: {0}", DateTime.Now));

            actual = target.ListarSubItemSaldoByAlmox(almoxarifado);

            System.Diagnostics.Debug.WriteLine(String.Format("2.0 - Tempo: {0}", DateTime.Now));

            DateTime dataFinal = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("Processamento: {0}", dataFinal - dataInicio));

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for ListarSubItemSaldoByAlmox
        ///</summary>
        [TestMethod()]
        public void ListarSubItemSaldoByAlmoxTest1()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int almoxarifado = 93; 
            int item = 49376; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            DateTime dataInicio = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("1.0 - Tempo: {0}", DateTime.Now));

            actual = target.ListarSubItemSaldoByAlmox(almoxarifado, item);

            System.Diagnostics.Debug.WriteLine(String.Format("2.0 - Tempo: {0}", DateTime.Now));

            DateTime dataFinal = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("Processamento: {0}", dataFinal - dataInicio));

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for SelectAlmox
        ///</summary>
        [TestMethod()]
        public void SelectAlmoxTest()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            //int _id = 20326; 
            int _id = 0; 
            int _idAlmoxarifado = 172; 
            
            SubItemMaterialEntity actual;

            DateTime dataInicio = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("1.0 - Tempo: {0}", DateTime.Now));

            actual = target.SelectAlmox(_id, _idAlmoxarifado);

            System.Diagnostics.Debug.WriteLine(String.Format("2.0 - Tempo: {0}", DateTime.Now));

            DateTime dataFinal = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("Processamento: {0}", dataFinal - dataInicio));

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for ListarSubItemAlmox
        ///</summary>
        [TestMethod()]
        public void ListarSubItemAlmoxTest()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int _itemId = 82343; 
            int _gestorId = 1; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            DateTime dataInicio = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("1.0 - Tempo: {0}", DateTime.Now));

            actual = target.ListarSubItemAlmox(_itemId, _gestorId);

            System.Diagnostics.Debug.WriteLine(String.Format("2.0 - Tempo: {0}", DateTime.Now));

            DateTime dataFinal = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("Processamento: {0}", dataFinal - dataInicio));

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for ListarSubItemPorAlmox
        ///</summary>
        [TestMethod()]
        public void ListarSubItemPorAlmoxTest()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int _itemId = 82343; 
            int _almoxarifadoId = 172; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            DateTime dataInicio = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("1.0 - Tempo: {0}", DateTime.Now));

            actual = target.ListarSubItemPorAlmox(_itemId, _almoxarifadoId);

            System.Diagnostics.Debug.WriteLine(String.Format("2.0 - Tempo: {0}", DateTime.Now));

            DateTime dataFinal = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(String.Format("Processamento: {0}", dataFinal - dataInicio));

            Assert.IsNotNull(actual);

        }

        ///// <summary>
        /////A test for ListarSubItemGestorPorPalavraChave
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("Sam.Domain.Infrastructure.dll")]
        //public void ListarSubItemGestorPorPalavraChaveTest()
        //{
        //    SubItemMaterialInfraestructure_Accessor target = new SubItemMaterialInfraestructure_Accessor(); 
        //    Nullable<long> Codigo = new Nullable<long>(); 
        //    string Descricao = string.Empty; 
        //    AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity(172); 
        //    almoxarifado.Gestor = new GestorEntity(1);
        //    IEnumerable<SubItemMaterialEntity> expected = null; 
        //    IEnumerable<SubItemMaterialEntity> actual;

        //    DateTime dataInicio = DateTime.Now;
        //    SystemWhiteLinePre(dataInicio);
        //    //actual = target.ListarSubItemGestorPorPalavraChave(Codigo, Descricao, almoxarifado);
        //    //SystemWhiteLinePos(actual, dataInicio);
            
        //   // Assert.IsNotNull(actual);
        //}

        ///// <summary>
        /////A test for ListarSubItemAlmoxPorPalavraChave
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("Sam.Domain.Infrastructure.dll")]
        //public void ListarSubItemAlmoxPorPalavraChaveTest()
        //{
        //    SubItemMaterialInfraestructure_Accessor target = new SubItemMaterialInfraestructure_Accessor(); 
        //    Nullable<long> Codigo = new Nullable<long>(); 
        //    string Descricao = string.Empty; 
        //    AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity(172); 
        //    almoxarifado.Gestor = new GestorEntity(1);
        //    IEnumerable<SubItemMaterialEntity> expected = null; 
        //    IEnumerable<SubItemMaterialEntity> actual;

        //    DateTime dataInicio = DateTime.Now;
        //    SystemWhiteLinePre(dataInicio);

        //    //actual = target.ListarSubItemAlmoxPorPalavraChave(Codigo, Descricao, almoxarifado);
        //    DateTime dataFinal = DateTime.Now;
        //    //SystemWhiteLinePos(actual, dataInicio);

        //    //Assert.IsNotNull(actual);
        //}

        /// <summary>
        ///A test for Listar
        ///</summary>
        [TestMethod()]
        public void ListarTest()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int _itemId = 0; 
            int _gestorId = 0; 
            IList<SubItemMaterialEntity> expected = null; 
            IEnumerable<SubItemMaterialEntity> actual;

            DateTime dataInicio = DateTime.Now;
            SystemWhiteLinePre(dataInicio);

            actual = target.Listar(_itemId, _gestorId);

            SystemWhiteLinePos(dataInicio);
            System.Diagnostics.Debug.WriteLine(actual.ToString());

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for Imprimir
        ///</summary>
        [TestMethod()]
        public void ImprimirTest()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int _itemId = 0; 
            int _gestorId = 0; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            DateTime dataInicio = DateTime.Now;
            SystemWhiteLinePre(dataInicio);
            actual = target.Imprimir(_itemId, _gestorId);

            SystemWhiteLinePos(dataInicio);
            System.Diagnostics.Debug.WriteLine(actual.ToString());
            System.Diagnostics.Debug.WriteLine(actual.Count);

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for Imprimir
        ///</summary>
        [TestMethod()]
        public void ImprimirTest1()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            target.Entity = new SubItemMaterialEntity(0);
            target.Entity.Gestor = new GestorEntity(0);


            DateTime dataInicio = DateTime.Now;
            SystemWhiteLinePre(dataInicio);

            actual = target.Imprimir();

            SystemWhiteLinePos(dataInicio);
            System.Diagnostics.Debug.WriteLine(actual.ToString());
            System.Diagnostics.Debug.WriteLine(actual.Count);
        }

        /// <summary>
        ///A test for Imprimir
        ///</summary>
        [TestMethod()]
        public void ImprimirTest2()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int _materialId = 0; 
            int _itemId = 0; 
            int _gestorId = 0; 
            int _almoxarifadoId = 0; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            DateTime dataInicio = DateTime.Now;
            SystemWhiteLinePre(dataInicio);

            actual = target.Imprimir(_materialId, _itemId, _gestorId, _almoxarifadoId);

            SystemWhiteLinePos(dataInicio);
            System.Diagnostics.Debug.WriteLine(actual.ToString());
            System.Diagnostics.Debug.WriteLine(actual.Count);
        }

        /// <summary>
        ///A test for ListarTodosCod
        ///</summary>
        [TestMethod()]
        public void ListarTodosCodTest()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            target.Entity = new SubItemMaterialEntity(0);
            target.Entity.Codigo = 0;
            target.Entity.ItemMaterial = new ItemMaterialEntity(0);
            target.Entity.ItemMaterial.Codigo = 0;

            DateTime dataInicio = DateTime.Now;
            SystemWhiteLinePre(dataInicio);

            actual = target.ListarTodosCod();

            SystemWhiteLinePos(dataInicio);
            System.Diagnostics.Debug.WriteLine(actual.ToString());
            System.Diagnostics.Debug.WriteLine(actual.Count);
        }

        /// <summary>
        ///A test for ListarSubItemMaterial
        ///</summary>
        [TestMethod()] //1seg
        public void ListarSubItemMaterialTest()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            Expression<Func<SubItemMaterialEntity, bool>> where = null; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            DateTime dataInicio = DateTime.Now;
            SystemWhiteLinePre(dataInicio);

            actual = target.ListarSubItemMaterial(a => a.Codigo == 0);

            SystemWhiteLinePos(dataInicio);
            System.Diagnostics.Debug.WriteLine(actual.ToString());
            System.Diagnostics.Debug.WriteLine(actual.Count);
        }

        /// <summary>
        ///A test for ListarTodosCodPorItemUgeAlmox
        ///</summary>
        [TestMethod()]
        public void ListarTodosCodPorItemUgeAlmoxTest()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int _itemCodigo = 0; 
            int _ugeId = 0; 
            int _almoxId = 0; 

            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            DateTime dataInicio = DateTime.Now;
            SystemWhiteLinePre(dataInicio);

            actual = target.ListarTodosCodPorItemUgeAlmox(_itemCodigo, _ugeId, _almoxId);

            SystemWhiteLinePos(dataInicio);
            System.Diagnostics.Debug.WriteLine(actual.ToString());
            System.Diagnostics.Debug.WriteLine(actual.Count);
        }

        /// <summary>
        ///A test for ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesa
        ///</summary>
        [TestMethod()]
        public void ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesaTest()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int _itemCodigo = 0; 
            int _ugeId = 0; 
            int _almoxId = 0;
            int _gestorId = 0;
            int _codigoNaturezaDespesa = 0; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            DateTime dataInicio = DateTime.Now;
            SystemWhiteLinePre(dataInicio);

            actual = target.ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesa(_itemCodigo, _ugeId, _almoxId, _gestorId, _codigoNaturezaDespesa);

            SystemWhiteLinePos(dataInicio);
            System.Diagnostics.Debug.WriteLine(actual.ToString());
            System.Diagnostics.Debug.WriteLine(actual.Count);
        }

        /// <summary>
        ///A test for ListarSubItemAlmox
        ///</summary>
        [TestMethod()]
        public void ListarSubItemAlmoxTest1()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int _materialId = 0; 
            int _itemId = 0; 
            int _gestorId = 0; 
            int _almoxarifadoId = 0;
            int _grupoId = 0;
            int _classeId = 0;
            Int64 _SubItemcodigo = 0;
            Int64 _Itemcodigo = 0;
            int _naturezaid= 0;
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            DateTime dataInicio = DateTime.Now;
            SystemWhiteLinePre(dataInicio);

            actual = target.ListarSubItemAlmox(_grupoId, _classeId, _materialId, _itemId, _gestorId, _almoxarifadoId, _naturezaid, _SubItemcodigo, _Itemcodigo);

            SystemWhiteLinePos(dataInicio);
            System.Diagnostics.Debug.WriteLine(actual.ToString());
            System.Diagnostics.Debug.WriteLine(actual.Count);
        }

        /// <summary>
        ///A test for ListarSubItemAlmox
        ///</summary>
        [TestMethod()]
        public void ListarSubItemAlmoxTest2()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int _itemId = 0; 
            int _gestorId = 1; 
            int _almoxarifadoId = 0; 
            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            DateTime dataInicio = DateTime.Now;
            SystemWhiteLinePre(dataInicio);

            actual = target.ListarSubItemAlmox(_itemId, _gestorId, _almoxarifadoId);

            SystemWhiteLinePos(dataInicio);
            System.Diagnostics.Debug.WriteLine(actual.ToString());
            System.Diagnostics.Debug.WriteLine(actual.Count);
        }

        /// <summary>
        ///A test for ListarCatalogoPorAlmoxarifadoNaturezaDespesa
        ///</summary>
        [TestMethod()]
        [Ignore()]
        public void ListarCatalogoPorAlmoxarifadoNaturezaDespesaTest()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int gestorId = 1;             
            int naturezaDespesaCodigo = 33903010; 

            int itemCodigo = 0;
            long subitemCodigo = 0;

            IList<SubItemMaterialEntity> expected = null; 
            IList<SubItemMaterialEntity> actual;

            actual = target.ListarCatalogoGestor(gestorId, naturezaDespesaCodigo, itemCodigo, subitemCodigo);

            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        //TODO: Criar Setups e Teardowns para os métodos, quando esses estiverem num contexto próprio.
        [TestMethod()]        
        public void SubItemMaterialInfraestructure_ListarCatalogoGestor_SelecionaListaSubItemMaterialEntity_PorNatureza_AListaNaoDeveEstarVazia()
        {
            var subItemMaterialInfraStructure = new SubItemMaterialInfraestructure();
            int gestor = 1;
            int naturezaDespesaCodigo = 33903010;
            int itemCodigo = 0;
            long subitemCodigo = 0;
            IList<SubItemMaterialEntity> actual;

            actual = subItemMaterialInfraStructure.ListarCatalogoGestor(gestor, naturezaDespesaCodigo, itemCodigo, subitemCodigo);

            Assert.IsTrue(actual.Count > 0);
        }

        [TestMethod()]
        public void SubItemMaterialInfraestructure_ListarCatalogoGestor_SelecionaListaSubItemMaterialEntity_PorSubitem_AListaNaoDeveEstarVazia()
        {
            var subItemMaterialInfraStructure = new SubItemMaterialInfraestructure();
            int gestor = 1;
            int naturezaDespesaCodigo = 0;
            int itemCodigo = 0;
            long subitemCodigo = 331000035521;
            IList<SubItemMaterialEntity> actual;

            actual = subItemMaterialInfraStructure.ListarCatalogoGestor(gestor, naturezaDespesaCodigo, itemCodigo, subitemCodigo);

            Assert.IsTrue(actual.Count > 0);
        }

        [TestMethod()]
        public void SubItemMaterialInfraestructure_ListarCatalogoGestor_SelecionaListaSubItemMaterialEntity_PorItem_AListaNaoDeveEstarVazia()
        {
            var subItemMaterialInfraStructure = new SubItemMaterialInfraestructure();
            int gestor = 1;
            int naturezaDespesaCodigo = 0;
            int itemCodigo = 3089533;
            long subitemCodigo = 0;
            IList<SubItemMaterialEntity> actual;

            actual = subItemMaterialInfraStructure.ListarCatalogoGestor(gestor, naturezaDespesaCodigo, itemCodigo, subitemCodigo);

            Assert.IsTrue(actual.Count > 0);
        }


        /// <summary>
        ///A test for SelectAlmox
        ///</summary>
        [TestMethod()]
        public void SelectAlmoxTest1()
        {
            SubItemMaterialInfraestructure target = new SubItemMaterialInfraestructure(); 
            int _id = 31817; 
            int _idAlmoxarifado = 130; 
            SubItemMaterialEntity expected = null; 
            SubItemMaterialEntity actual;
            actual = target.SelectAlmox(_id, _idAlmoxarifado);
        }
    }
}
