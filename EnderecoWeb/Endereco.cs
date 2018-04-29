using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace EnderecoWeb
{
    public enum TiposdeBusca { Logradouro, Cep }

    
    public class Endereco
    {
        //->Variaveis
        static string vSite = "http://cep.republicavirtual.com.br/web_cep.php?cep=";
        static string vFormato = "&formato=xml";
        static string vCep;
        static string vTipoLogradouro;
        static string vLogradouro;
        static string vBairro;
        static string vCidade;
        static string vUF;
        static int vResultado;
        static string vMensagem;
        static Dictionary<int, string> vMensagens;
        static DataSet vTabelas;

        //->Propriedades
        public static string Cep => string.IsNullOrEmpty(vCep) ? "" : vCep;
        public static string TipoLogradouro => string.IsNullOrEmpty(vTipoLogradouro) ? "" : vTipoLogradouro;
        public static string Logradouro => string.IsNullOrEmpty(vLogradouro) ? "" : vLogradouro;
        public static string Bairro => string.IsNullOrEmpty(vBairro) ? "" : vLogradouro;
        public static string Cidade  => string.IsNullOrEmpty(vBairro) ? "" : vLogradouro;
        public static string UF => string.IsNullOrEmpty(vBairro) ? "" : vLogradouro; 
        public static string Mensagem => string.IsNullOrEmpty(vMensagem) ? "" : vMensagem;

        public static object[] ArraydeEndereco => new object[] { vCep, vTipoLogradouro, vLogradouro, vBairro, vCidade, vUF };


        //->Construtores
        public Endereco() { }

        public Endereco(TiposdeBusca tipo, string valor)
        {
            CarregarMensagens();

            switch (tipo)
            {
                case TiposdeBusca.Logradouro:
                    vLogradouro = valor;
                    MessageBox.Show("Em Contrução! Use tipo Cep.");
                    break;

                case TiposdeBusca.Cep:
                    vCep = valor.Replace("-", "").Trim();
                    BuscaViaCep();
                    break;

                default:
                    break;
            }
        }
        


        public static void BuscaViaCep()
        {
            try
            {
                if (TratarCep(vCep))
                {
                    vTabelas = new DataSet("Enderecos");
                    vTabelas.ReadXml($"{vSite}{vCep}{vFormato}");
                    
                    if (vTabelas != null || vTabelas.Tables[0].Rows.Count > 0)
                    {
                        vResultado = Convert.ToInt32(vTabelas.Tables[0].Rows[0]["resultado"]);

                        switch (vResultado)
                        {
                            case 1:
                                vTipoLogradouro = vTabelas.Tables[0].Rows[0]["tipo_logradouro"].ToString();
                                vLogradouro = vTabelas.Tables[0].Rows[0]["logradouro"].ToString();
                                vBairro = vTabelas.Tables[0].Rows[0]["bairro"].ToString();
                                vCidade = vTabelas.Tables[0].Rows[0]["ciadde"].ToString();
                                vUF = vTabelas.Tables[0].Rows[0]["uf"].ToString();
                                vMensagem = vMensagens[1];
                                break;

                            case 2:
                                vTipoLogradouro = string.Empty;
                                vLogradouro = string.Empty;
                                vBairro = string.Empty;
                                vCidade = vTabelas.Tables[0].Rows[0]["ciadde"].ToString();
                                vUF = vTabelas.Tables[0].Rows[0]["uf"].ToString();
                                vMensagem = vMensagens[2];
                                break;

                            default:
                                vTipoLogradouro = string.Empty;
                                vLogradouro = string.Empty;
                                vBairro = string.Empty;
                                vCidade = string.Empty;
                                vUF = string.Empty;
                                vMensagem = vMensagens[0];
                                break;
                        }
                    }
                }                
            }
            catch (Exception erro)
            {
                MessageBox.Show($"Ocorreu um erro: {erro.Message}");
            }
        }

        protected static bool TratarCep(string valor)
        {
            try
            {
                if (!valor.Length.Equals(8))
                    throw new Exception("O número do Cep informardo é inválido! Um cep válido contém 8 dígitos mais um sinal '-'(opcional).", new ArgumentOutOfRangeException());
                else
                {
                    foreach (char digito in valor)
                        if (!char.IsDigit(digito) || !char.IsNumber(digito))
                            throw new Exception("O número do Cep informardo é inválido! Um cep válido só possuem números ou dígitos.", new FormatException());

                    return true;
                }
            }
            catch (Exception erro)
            {
                MessageBox.Show($"Ocorreu um erro: {erro.Message}");
                return false;
            }
        }
        

        protected static void CarregarMensagens()
        {
            vMensagens = new Dictionary<int, string>();
            vMensagens.Add(0, "Cep não encontrado!");
            vMensagens.Add(1, "Endereço Completo encontrado!");
            vMensagens.Add(2, "Endereço Único encontrado!");
        }
    }
}
