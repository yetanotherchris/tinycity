class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "2.4.0"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.4.0/tinycity-v2.4.0-osx-arm64"
      sha256 "955ef6290f420ba13608027fb9eb5899b6671e80362a1b373edab9d915d8c0da"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.4.0/tinycity-v2.4.0-osx-x64"
      sha256 "5f948433b76efc1075baf001b4e323824d7d2166b8a170ef4b394ee9979aa2a1"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v2.4.0/tinycity-v2.4.0-linux-x64"
    sha256 "ca7000c5bcf1bbf1338a73c6fbfe804d5618797cac78e40aa768ab2b7044fc34"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v2.4.0-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v2.4.0-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v2.4.0-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end












