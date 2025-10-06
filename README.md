KeyboardShortcut (QuickLaunch Bar)
Um utilitário leve para Windows 11 que fornece uma barra de atalhos rápida para seus aplicativos favoritos, projetado para ser acessado pelo teclado.




Inicie um aplicativo: Clique no ícone desejado ou pressione a tecla numérica correspondente (1, 2, 3...).

Desaparecimento Automático: A barra se fechará automaticamente após iniciar um programa ou se você clicar fora dela (perder o foco).

Configuração (menu.json)
Para configurar seus atalhos, edite o arquivo menu.json que se encontra no mesmo diretório do executável. Se o arquivo não existir, ele será criado com exemplos na primeira execução.

Slot: O número (de 1 a 9) que será associado ao atalho.
Name: O nome do aplicativo (usado para referência).
Path: O caminho completo para o executável ou um comando que possa ser encontrado no PATH do sistema.
Exemplo de menu.json
[
  {
    "Slot": 1,
    "Name": "Visual Studio Code",
    "Path": "C:\\Users\\SeuUsuario\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe"
  },
  {
    "Slot": 2,
    "Name": "Windows Terminal",
    "Path": "wt.exe"
  },
  {
    "Slot": 3,
    "Name": "Explorador de Arquivos",
    "Path": "explorer.exe"
  }
]


### Requisitos para Compilação
Para compilar este projeto, você precisará do seguinte ambiente:
- IDE: Visual Studio 2022
**SDKs:**
- Windows App SDK
- SDK do .NET correspondente
- Framework de UI: WinUI 3
**Pacotes NuGet:**
- WinUIEx
- System.Drawing.Common


#### Limitações conhecidas: Programas Store não é possível recuperar o icone como faço com programas tradicionais, o mesmo ficará sem icone.
