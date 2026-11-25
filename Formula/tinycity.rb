class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "11.0.0"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v11.0.0/tinycity-v11.0.0-osx-arm64"
      sha256 "9b7e606be8b70d1be7ec9b5b2e260ba9a09e4023c7bcdbcfa238d24769339a6b"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v11.0.0/tinycity-v11.0.0-osx-x64"
      sha256 "28fcd5fcccae789da743428393ab09c602ffea6a618c271eafb4c78939bd4305"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v11.0.0/tinycity-v11.0.0-linux-x64"
    sha256 "5392a61d500d81b1c8b345fc68682c18a3590d5380a97ec817d5999eae49b8e8"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v11.0.0-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v11.0.0-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v11.0.0-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end






















