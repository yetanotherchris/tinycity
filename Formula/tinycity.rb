class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "3.2.1"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.2.1/tinycity-v3.2.1-osx-arm64"
      sha256 "49cf2bbdd2eb014cb06f0063b1bbb2f1aee117fd67d640cc85ddf3a99f0b43ee"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.2.1/tinycity-v3.2.1-osx-x64"
      sha256 "97454485e3c0fbb06b0efbaedbf6838af8de41fe1af0dbc474cfd8ecae491551"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v3.2.1/tinycity-v3.2.1-linux-x64"
    sha256 "9e4c5f2e9cfe8500138b96200282c5bc81a9331f81741055521a6cb75bb433f8"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v3.2.1-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v3.2.1-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v3.2.1-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end

















