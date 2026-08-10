class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "3.4.1"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.4.1/tinycity-v3.4.1-osx-arm64"
      sha256 "0ce28907fe3552a595f88b51471318da67841c8b14e56627775beac461f4d199"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.4.1/tinycity-v3.4.1-osx-x64"
      sha256 "dd12d7525c32b4a2c7fa9ab8cd7d0f7bb33615cc97823822e167255af5fe39b9"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v3.4.1/tinycity-v3.4.1-linux-x64"
    sha256 "71d8fb60ceae3032ddca07c2514e26de3480014604fb487e7c88bcce445caf62"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v3.4.1-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v3.4.1-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v3.4.1-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end




















