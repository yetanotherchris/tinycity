class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "2.0.3"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.0.3/tinycity-v2.0.3-osx-arm64"
      sha256 "6a2c6a456509ae63564758e0bbbae2e9a015dff1cd968d3ad4f5de865e72779c"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.0.3/tinycity-v2.0.3-osx-x64"
      sha256 "540f6d8767505f11f8286901b831b259c2ed6c9dc600d94c8d4d17a2fbe728e2"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v2.0.3/tinycity-v2.0.3-linux-x64"
    sha256 "d26bd7a9f945f55c8395b5950f9e5afe236123d33c0a16901042618e591e0d00"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v2.0.3-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v2.0.3-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v2.0.3-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end





