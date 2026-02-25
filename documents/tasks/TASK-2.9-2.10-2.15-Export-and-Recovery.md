# TASK LOTE 1 (Delivery 2b) - Exportação e Resiliência

## Tarefas Incluídas:
- **2.9 Audio Export:** Exportar gravação em formato WAV/MP3 usando o diálogo padrão de salvar do Avalonia.
- **2.10 Text Export:** Exportar transcrição em TXT ou Markdown.
- **2.15 Crash Recovery:** Implementar auto-save do buffer temporário durante a gravação. Se o app fechar de forma abrupta, na próxima inicialização ele deve detectar o buffer órfão e perguntar ao usuário se deseja recuperar.

## Instruções para o Elfo (Claude Code)
1. Crie os serviços `ExportService` (para áudio e texto) na camada `Application` ou `Infrastructure` correspondente.
2. Atualize o `RecordingViewModel` para suportar comandos de Export Audio e Export Text, vinculando-os ao UI.
3. Para o Crash Recovery: o `AudioCaptureService` deve escrever os chunks do microfone em um arquivo físico temporário (.raw ou .wav temporário) em background, em vez de guardar tudo só na RAM. 
4. No boot da aplicação, o `Bootstrapper` ou `MainViewModel` deve verificar o diretório temporário e, se encontrar lixo, instanciar uma notificação de recuperação.
5. Siga a arquitetura limpa e adicione testes (xUnit).
