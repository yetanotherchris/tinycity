class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "2.2.1"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.2.1/tinycity-v2.2.1-osx-arm64"
      sha256 "e9a9770deef648592a6f092ea8964cef669c94a9fa3929b9f12159689fee842c"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.2.1/tinycity-v2.2.1-osx-x64"
      sha256 "7aed4c6d5af233344643439292b7e105fc042e566ccfbac724144e065278ce06"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v2.2.1/tinycity-v2.2.1-linux-x64"
    sha256 "617c4a4ce515b704d284a8043ef1c97478eae5d2e578955309b442ca3c0e7a34"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v2.2.1-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v2.2.1-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v2.2.1-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end










