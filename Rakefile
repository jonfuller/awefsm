require 'rubygems'
require 'albacore'
require 'fileutils'

include FileUtils

task :default => [:build]

desc "Build AweFsm"
msbuild :build do |msb|
  msb.properties :configuration => :Debug, :TrackFileAccess => :false
  msb.targets :Build
  msb.solution = "src/Awefsm.sln"
end

desc "Builds, runs tests, transforms mspec output"
task :test => [:build, :mspec]

mspec do |mspec|
  mspec.command = 'lib/mspec/mspec.exe'
  mspec.assemblies = []
  mspec.html_output = 'output/mspec.html'
  mspec.options '--xml output/mspec.xml'
end
