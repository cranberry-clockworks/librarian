defmodule Librarian.Scribe do
  require EEx

  def main(argv) do
    with {:ok, template_path} <- parse_arguments(argv),
         {:ok, template} <- File.read(template_path),
         input <- IO.read(:stdio, :eof),
         {:ok, bind} <- Jason.decode(input) do
      EEx.eval_string(template, assigns: bind) |> IO.puts()
    end
  end

  defp parse_arguments(argv) do
    case OptionParser.parse(argv, strict: [template: :string]) do
      {[template: path], _, []} -> {:ok, path}
      {_, _, [errors]} -> {:error, errors}
      _ -> {:error, argv}
    end
  end
end
